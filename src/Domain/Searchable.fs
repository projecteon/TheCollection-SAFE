namespace Domain

open System
open System.Reflection
open Domain.ReflectionUtil

module Searchable =
    // https://stackoverflow.com/questions/38257283/in-f-is-there-a-shortcut-for-creating-a-record-type-with-a-primary-key-used-fo
    [<AttributeUsage(AttributeTargets.All, AllowMultiple = true)>]
    type SearchableAttribute() = inherit Attribute()

    type Type with
        member x.IsSimpleType() =
                x.GetTypeInfo().IsValueType ||
                x.GetTypeInfo().IsPrimitive ||
                List.contains x [
                    typeof<String>
                    typeof<Decimal>
                    typeof<DateTime>
                    typeof<DateTimeOffset>
                    typeof<TimeSpan>
                    typeof<Guid>
                ] ||
                Convert.GetTypeCode(x) <> TypeCode.Object

    let getSearchableProperties(objectValue: obj) =
        if objectValue = null then Seq.empty
        else
            objectValue.GetType().GetProperties()
            |> Seq.where (fun p ->
                let attr = p.GetCustomAttributes(typeof<SearchableAttribute>, true)
                attr.Length > 0
            )

    let getSearchablePropertyValue<'T> (propertyInfo: PropertyInfo) (objectValue: 'T) =
        match propertyInfo.PropertyType, propertyInfo.GetValue(objectValue) with
        | pi, NotNull  when pi.IsSimpleType() -> propertyInfo.GetValue(objectValue)
        | pi, NotNull -> propertyInfo.GetValue(objectValue)
        | _,_ -> null


    type TreeNode =
    | LeafNode of string
    | BranchNode of TreeNode seq

    type Tree =
      TreeNode seq

    let rec createSearchableNode (o: obj) =
        getSearchableProperties(o)
        |> Seq.map (fun x ->
                        match (isOption x) with
                        | false ->
                            match x with
                            | y when y.PropertyType.IsSimpleType() -> TreeNode.LeafNode (y.GetValue(o).ToString())
                            | _ -> TreeNode.BranchNode (createSearchableNode (x.GetValue(o)))
                        | true ->
                            let unboxedValue = unboxOptionValue(x.GetValue(o))
                            match unboxedValue with
                            | null -> TreeNode.BranchNode (createSearchableNode (unboxedValue))
                            | b when b.GetType().IsSimpleType() -> TreeNode.LeafNode (unboxedValue.ToString())
                            | _ -> TreeNode.BranchNode (createSearchableNode (unboxOptionValue(x.GetValue(o))))
                    )

    let flattenTree (tree: TreeNode list) =
        let rec loop acc (nodes: TreeNode list) =
            match nodes with
            | [] -> acc
            | x::xs ->
                match x with
                    | LeafNode l -> loop (l::acc) xs
                    | BranchNode b -> loop ((loop acc (b |> Seq.toList))) xs

        loop [] tree