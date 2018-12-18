namespace Domain

open System.Reflection

module ReflectionUtil =
  let unboxOptionValue (objectValue: obj) =
      if objectValue = null then objectValue
      else
          let ps = objectValue.GetType().GetProperties()
          let isSomeProp = ps |> Seq.tryFind (fun x -> x.Name = "IsSome")
          let valueProp = ps |> Seq.tryFind (fun x -> x.Name = "Value")
          match valueProp, isSomeProp with
          | Some x, Some y -> x.GetValue(objectValue)
          | _, _ -> objectValue

  let (|NotNull|_|) value =
      if obj.ReferenceEquals(value, null) then None
      else Some()

  // https://stackoverflow.com/questions/3151099/is-there-a-way-in-f-to-type-test-against-a-generic-type-without-specifying-the
  // https://stackoverflow.com/questions/24841185/how-to-deal-with-option-values-generically-in-f
  let isOption (p:PropertyInfo) =
      p.PropertyType.IsGenericType &&
      p.PropertyType.GetGenericTypeDefinition() = typedefof<Option<_>>