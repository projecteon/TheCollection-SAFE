// ts2fable 0.6.1
module rec Fable.Import.Moment
open System
open Fable.Core
open Fable.Import.JS

let [<Import("*","module")>] moment: Moment.IExports = jsNative

type IExports = Moment.MomentInput -> Moment.Moment

module Moment =
    type [<AllowNullLiteral>] IExports =
        abstract version: string
        abstract fn: Moment
        abstract utc: ?inp: MomentInput * ?format: MomentFormatSpecification * ?strict: bool -> Moment
        abstract utc: ?inp: MomentInput * ?format: MomentFormatSpecification * ?language: string * ?strict: bool -> Moment
        abstract unix: timestamp: float -> Moment
        abstract invalid: ?flags: MomentParsingFlagsOpt -> Moment
        abstract isMoment: m: obj option -> bool
        abstract isDate: m: obj option -> bool
        abstract isDuration: d: obj option -> bool
        abstract lang: ?language: string -> string
        abstract lang: ?language: string * ?definition: Locale -> string
        abstract locale: ?language: string -> string
        abstract locale: ?language: ResizeArray<string> -> string
        abstract locale: ?language: string * ?definition: U2<LocaleSpecification, unit> -> string
        abstract localeData: ?key: U2<string, ResizeArray<string>> -> Locale
        abstract duration: ?inp: DurationInputArg1 * ?unit: DurationInputArg2 -> Duration
        abstract parseZone: ?inp: MomentInput * ?format: MomentFormatSpecification * ?strict: bool -> Moment
        abstract parseZone: ?inp: MomentInput * ?format: MomentFormatSpecification * ?language: string * ?strict: bool -> Moment
        abstract months: unit -> ResizeArray<string>
        abstract months: index: float -> string
        abstract months: format: string -> ResizeArray<string>
        abstract months: format: string * index: float -> string
        abstract monthsShort: unit -> ResizeArray<string>
        abstract monthsShort: index: float -> string
        abstract monthsShort: format: string -> ResizeArray<string>
        abstract monthsShort: format: string * index: float -> string
        abstract weekdays: unit -> ResizeArray<string>
        abstract weekdays: index: float -> string
        abstract weekdays: format: string -> ResizeArray<string>
        abstract weekdays: format: string * index: float -> string
        abstract weekdays: localeSorted: bool -> ResizeArray<string>
        abstract weekdays: localeSorted: bool * index: float -> string
        abstract weekdays: localeSorted: bool * format: string -> ResizeArray<string>
        abstract weekdays: localeSorted: bool * format: string * index: float -> string
        abstract weekdaysShort: unit -> ResizeArray<string>
        abstract weekdaysShort: index: float -> string
        abstract weekdaysShort: format: string -> ResizeArray<string>
        abstract weekdaysShort: format: string * index: float -> string
        abstract weekdaysShort: localeSorted: bool -> ResizeArray<string>
        abstract weekdaysShort: localeSorted: bool * index: float -> string
        abstract weekdaysShort: localeSorted: bool * format: string -> ResizeArray<string>
        abstract weekdaysShort: localeSorted: bool * format: string * index: float -> string
        abstract weekdaysMin: unit -> ResizeArray<string>
        abstract weekdaysMin: index: float -> string
        abstract weekdaysMin: format: string -> ResizeArray<string>
        abstract weekdaysMin: format: string * index: float -> string
        abstract weekdaysMin: localeSorted: bool -> ResizeArray<string>
        abstract weekdaysMin: localeSorted: bool * index: float -> string
        abstract weekdaysMin: localeSorted: bool * format: string -> ResizeArray<string>
        abstract weekdaysMin: localeSorted: bool * format: string * index: float -> string
        //abstract min: moments: ResizeArray<Moment> -> Moment
        abstract min: [<ParamArray>] moments: ResizeArray<Moment> -> Moment
        //abstract max: moments: ResizeArray<Moment> -> Moment
        abstract max: [<ParamArray>] moments: ResizeArray<Moment> -> Moment
        /// Returns unix time in milliseconds. Overwrite for profit.
        abstract now: unit -> float
        abstract defineLocale: language: string * localeSpec: U2<LocaleSpecification, unit> -> Locale
        abstract updateLocale: language: string * localeSpec: U2<LocaleSpecification, unit> -> Locale
        abstract locales: unit -> ResizeArray<string>
        abstract normalizeUnits: unit: UnitOfTime.All -> string
        abstract relativeTimeThreshold: threshold: string -> U2<float, bool>
        abstract relativeTimeThreshold: threshold: string * limit: float -> bool
        abstract relativeTimeRounding: fn: (float -> float) -> bool
        abstract relativeTimeRounding: unit -> (float -> float)
        abstract calendarFormat: m: Moment * now: Moment -> string
        abstract parseTwoDigitYear: input: string -> float
        abstract ISO_8601: MomentBuiltinFormat
        abstract RFC_2822: MomentBuiltinFormat
        abstract defaultFormat: string
        abstract defaultFormatUtc: string

    type [<StringEnum>] [<RequireQualifiedAccess>] RelativeTimeKey =
        | S
        | Ss
        | Mm
        | H
        | Hh
        | D
        | Dd
        | [<CompiledName "M">] M
        | [<CompiledName "MM">] MM
        | Y
        | Yy

    type CalendarKey =
        U7<string, string, string, string, string, string, string>

    [<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module CalendarKey =
        let ofCase1 v: CalendarKey = v |> U7.Case1
        let isCase1 (v: CalendarKey) = match v with U7.Case1 _ -> true | _ -> false
        let asCase1 (v: CalendarKey) = match v with U7.Case1 o -> Some o | _ -> None
        let ofCase2 v: CalendarKey = v |> U7.Case2
        let isCase2 (v: CalendarKey) = match v with U7.Case2 _ -> true | _ -> false
        let asCase2 (v: CalendarKey) = match v with U7.Case2 o -> Some o | _ -> None
        let ofCase3 v: CalendarKey = v |> U7.Case3
        let isCase3 (v: CalendarKey) = match v with U7.Case3 _ -> true | _ -> false
        let asCase3 (v: CalendarKey) = match v with U7.Case3 o -> Some o | _ -> None
        let ofCase4 v: CalendarKey = v |> U7.Case4
        let isCase4 (v: CalendarKey) = match v with U7.Case4 _ -> true | _ -> false
        let asCase4 (v: CalendarKey) = match v with U7.Case4 o -> Some o | _ -> None
        let ofCase5 v: CalendarKey = v |> U7.Case5
        let isCase5 (v: CalendarKey) = match v with U7.Case5 _ -> true | _ -> false
        let asCase5 (v: CalendarKey) = match v with U7.Case5 o -> Some o | _ -> None
        let ofCase6 v: CalendarKey = v |> U7.Case6
        let isCase6 (v: CalendarKey) = match v with U7.Case6 _ -> true | _ -> false
        let asCase6 (v: CalendarKey) = match v with U7.Case6 o -> Some o | _ -> None
        let ofString v: CalendarKey = v |> U7.Case7
        let isString (v: CalendarKey) = match v with U7.Case7 _ -> true | _ -> false
        let asString (v: CalendarKey) = match v with U7.Case7 o -> Some o | _ -> None

    type [<StringEnum>] [<RequireQualifiedAccess>] LongDateFormatKey =
        | [<CompiledName "LTS">] LTS
        | [<CompiledName "LT">] LT
        | [<CompiledName "L">] L
        | [<CompiledName "LL">] LL
        | [<CompiledName "LLL">] LLL
        | [<CompiledName "LLLL">] LLLL
        | Lts
        | Lt
        | Ll
        | Lll
        | Llll

    type [<AllowNullLiteral>] Locale =
        abstract calendar: ?key: CalendarKey * ?m: Moment * ?now: Moment -> string
        abstract longDateFormat: key: LongDateFormatKey -> string
        abstract invalidDate: unit -> string
        abstract ordinal: n: float -> string
        abstract preparse: inp: string -> string
        abstract postformat: inp: string -> string
        abstract relativeTime: n: float * withoutSuffix: bool * key: RelativeTimeKey * isFuture: bool -> string
        abstract pastFuture: diff: float * absRelTime: string -> string
        abstract set: config: Object -> unit
        abstract months: unit -> ResizeArray<string>
        abstract months: m: Moment * ?format: string -> string
        abstract monthsShort: unit -> ResizeArray<string>
        abstract monthsShort: m: Moment * ?format: string -> string
        abstract monthsParse: monthName: string * format: string * strict: bool -> float
        abstract monthsRegex: strict: bool -> RegExp
        abstract monthsShortRegex: strict: bool -> RegExp
        abstract week: m: Moment -> float
        abstract firstDayOfYear: unit -> float
        abstract firstDayOfWeek: unit -> float
        abstract weekdays: unit -> ResizeArray<string>
        abstract weekdays: m: Moment * ?format: string -> string
        abstract weekdaysMin: unit -> ResizeArray<string>
        abstract weekdaysMin: m: Moment -> string
        abstract weekdaysShort: unit -> ResizeArray<string>
        abstract weekdaysShort: m: Moment -> string
        abstract weekdaysParse: weekdayName: string * format: string * strict: bool -> float
        abstract weekdaysRegex: strict: bool -> RegExp
        abstract weekdaysShortRegex: strict: bool -> RegExp
        abstract weekdaysMinRegex: strict: bool -> RegExp
        abstract isPM: input: string -> bool
        abstract meridiem: hour: float * minute: float * isLower: bool -> string

    type [<AllowNullLiteral>] StandaloneFormatSpec =
        abstract format: ResizeArray<string> with get, set
        abstract standalone: ResizeArray<string> with get, set
        abstract isFormat: RegExp option with get, set

    type [<AllowNullLiteral>] WeekSpec =
        abstract dow: float with get, set
        abstract doy: float with get, set

    type CalendarSpecVal =
        U2<string, (MomentInput -> Moment -> string)>

    [<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module CalendarSpecVal =
        let ofString v: CalendarSpecVal = v |> U2.Case1
        let isString (v: CalendarSpecVal) = match v with U2.Case1 _ -> true | _ -> false
        let asString (v: CalendarSpecVal) = match v with U2.Case1 o -> Some o | _ -> None
        let ofCase2 v: CalendarSpecVal = v |> U2.Case2
        let isCase2 (v: CalendarSpecVal) = match v with U2.Case2 _ -> true | _ -> false
        let asCase2 (v: CalendarSpecVal) = match v with U2.Case2 o -> Some o | _ -> None

    type [<AllowNullLiteral>] CalendarSpec =
        abstract sameDay: CalendarSpecVal option with get, set
        abstract nextDay: CalendarSpecVal option with get, set
        abstract lastDay: CalendarSpecVal option with get, set
        abstract nextWeek: CalendarSpecVal option with get, set
        abstract lastWeek: CalendarSpecVal option with get, set
        abstract sameElse: CalendarSpecVal option with get, set
        [<Emit "$0[$1]{{=$2}}">] abstract Item: x: string -> U2<CalendarSpecVal, unit> with get, set

    type RelativeTimeSpecVal =
        U2<string, (float -> bool -> RelativeTimeKey -> bool -> string)>

    [<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module RelativeTimeSpecVal =
        let ofString v: RelativeTimeSpecVal = v |> U2.Case1
        let isString (v: RelativeTimeSpecVal) = match v with U2.Case1 _ -> true | _ -> false
        let asString (v: RelativeTimeSpecVal) = match v with U2.Case1 o -> Some o | _ -> None
        let ofCase2 v: RelativeTimeSpecVal = v |> U2.Case2
        let isCase2 (v: RelativeTimeSpecVal) = match v with U2.Case2 _ -> true | _ -> false
        let asCase2 (v: RelativeTimeSpecVal) = match v with U2.Case2 o -> Some o | _ -> None

    type RelativeTimeFuturePastVal =
        U2<string, (string -> string)>

    [<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module RelativeTimeFuturePastVal =
        let ofString v: RelativeTimeFuturePastVal = v |> U2.Case1
        let isString (v: RelativeTimeFuturePastVal) = match v with U2.Case1 _ -> true | _ -> false
        let asString (v: RelativeTimeFuturePastVal) = match v with U2.Case1 o -> Some o | _ -> None
        let ofCase2 v: RelativeTimeFuturePastVal = v |> U2.Case2
        let isCase2 (v: RelativeTimeFuturePastVal) = match v with U2.Case2 _ -> true | _ -> false
        let asCase2 (v: RelativeTimeFuturePastVal) = match v with U2.Case2 o -> Some o | _ -> None

    type [<AllowNullLiteral>] RelativeTimeSpec =
        abstract future: RelativeTimeFuturePastVal with get, set
        abstract past: RelativeTimeFuturePastVal with get, set
        abstract s: RelativeTimeSpecVal with get, set
        abstract ss: RelativeTimeSpecVal with get, set
        abstract m: RelativeTimeSpecVal with get, set
        abstract mm: RelativeTimeSpecVal with get, set
        abstract h: RelativeTimeSpecVal with get, set
        abstract hh: RelativeTimeSpecVal with get, set
        abstract d: RelativeTimeSpecVal with get, set
        abstract dd: RelativeTimeSpecVal with get, set
        abstract M: RelativeTimeSpecVal with get, set
        abstract MM: RelativeTimeSpecVal with get, set
        abstract y: RelativeTimeSpecVal with get, set
        abstract yy: RelativeTimeSpecVal with get, set

    type [<AllowNullLiteral>] LongDateFormatSpec =
        abstract LTS: string with get, set
        abstract LT: string with get, set
        abstract L: string with get, set
        abstract LL: string with get, set
        abstract LLL: string with get, set
        abstract LLLL: string with get, set
        abstract lts: string option with get, set
        abstract lt: string option with get, set
        abstract l: string option with get, set
        abstract ll: string option with get, set
        abstract lll: string option with get, set
        abstract llll: string option with get, set

    type [<AllowNullLiteral>] MonthWeekdayFn =
        [<Emit "$0($1...)">] abstract Invoke: momentToFormat: Moment * ?format: string -> string

    type [<AllowNullLiteral>] WeekdaySimpleFn =
        [<Emit "$0($1...)">] abstract Invoke: momentToFormat: Moment -> string

    type [<AllowNullLiteral>] LocaleSpecification =
        abstract months: U3<ResizeArray<string>, StandaloneFormatSpec, MonthWeekdayFn> option with get, set
        abstract monthsShort: U3<ResizeArray<string>, StandaloneFormatSpec, MonthWeekdayFn> option with get, set
        abstract weekdays: U3<ResizeArray<string>, StandaloneFormatSpec, MonthWeekdayFn> option with get, set
        abstract weekdaysShort: U3<ResizeArray<string>, StandaloneFormatSpec, WeekdaySimpleFn> option with get, set
        abstract weekdaysMin: U3<ResizeArray<string>, StandaloneFormatSpec, WeekdaySimpleFn> option with get, set
        abstract meridiemParse: RegExp option with get, set
        abstract meridiem: (float -> float -> bool -> string) option with get, set
        abstract isPM: (string -> bool) option with get, set
        abstract longDateFormat: LongDateFormatSpec option with get, set
        abstract calendar: CalendarSpec option with get, set
        abstract relativeTime: RelativeTimeSpec option with get, set
        abstract invalidDate: string option with get, set
        abstract ordinal: (float -> string) option with get, set
        abstract ordinalParse: RegExp option with get, set
        abstract week: WeekSpec option with get, set
        [<Emit "$0[$1]{{=$2}}">] abstract Item: x: string -> obj option with get, set

    type [<AllowNullLiteral>] MomentObjectOutput =
        abstract years: float with get, set
        abstract months: float with get, set
        abstract date: float with get, set
        abstract hours: float with get, set
        abstract minutes: float with get, set
        abstract seconds: float with get, set
        abstract milliseconds: float with get, set

    type [<AllowNullLiteral>] Duration =
        abstract clone: unit -> Duration
        abstract humanize: ?withSuffix: bool -> string
        abstract abs: unit -> Duration
        abstract ``as``: units: UnitOfTime.Base -> float
        abstract get: units: UnitOfTime.Base -> float
        abstract milliseconds: unit -> float
        abstract asMilliseconds: unit -> float
        abstract seconds: unit -> float
        abstract asSeconds: unit -> float
        abstract minutes: unit -> float
        abstract asMinutes: unit -> float
        abstract hours: unit -> float
        abstract asHours: unit -> float
        abstract days: unit -> float
        abstract asDays: unit -> float
        abstract weeks: unit -> float
        abstract asWeeks: unit -> float
        abstract months: unit -> float
        abstract asMonths: unit -> float
        abstract years: unit -> float
        abstract asYears: unit -> float
        abstract add: ?inp: DurationInputArg1 * ?unit: DurationInputArg2 -> Duration
        abstract subtract: ?inp: DurationInputArg1 * ?unit: DurationInputArg2 -> Duration
        abstract locale: unit -> string
        abstract locale: locale: LocaleSpecifier -> Duration
        abstract localeData: unit -> Locale
        abstract toISOString: unit -> string
        abstract toJSON: unit -> string
        abstract lang: locale: LocaleSpecifier -> Moment
        abstract lang: unit -> Locale
        abstract toIsoString: unit -> string

    type [<AllowNullLiteral>] MomentRelativeTime =
        abstract future: obj option with get, set
        abstract past: obj option with get, set
        abstract s: obj option with get, set
        abstract ss: obj option with get, set
        abstract m: obj option with get, set
        abstract mm: obj option with get, set
        abstract h: obj option with get, set
        abstract hh: obj option with get, set
        abstract d: obj option with get, set
        abstract dd: obj option with get, set
        abstract M: obj option with get, set
        abstract MM: obj option with get, set
        abstract y: obj option with get, set
        abstract yy: obj option with get, set

    type [<AllowNullLiteral>] MomentLongDateFormat =
        abstract L: string with get, set
        abstract LL: string with get, set
        abstract LLL: string with get, set
        abstract LLLL: string with get, set
        abstract LT: string with get, set
        abstract LTS: string with get, set
        abstract l: string option with get, set
        abstract ll: string option with get, set
        abstract lll: string option with get, set
        abstract llll: string option with get, set
        abstract lt: string option with get, set
        abstract lts: string option with get, set

    type [<AllowNullLiteral>] MomentParsingFlags =
        abstract empty: bool with get, set
        abstract unusedTokens: ResizeArray<string> with get, set
        abstract unusedInput: ResizeArray<string> with get, set
        abstract overflow: float with get, set
        abstract charsLeftOver: float with get, set
        abstract nullInput: bool with get, set
        abstract invalidMonth: U2<string, unit> with get, set
        abstract invalidFormat: bool with get, set
        abstract userInvalidated: bool with get, set
        abstract iso: bool with get, set
        abstract parsedDateParts: ResizeArray<obj option> with get, set
        abstract meridiem: U2<string, unit> with get, set

    type [<AllowNullLiteral>] MomentParsingFlagsOpt =
        abstract empty: bool option with get, set
        abstract unusedTokens: ResizeArray<string> option with get, set
        abstract unusedInput: ResizeArray<string> option with get, set
        abstract overflow: float option with get, set
        abstract charsLeftOver: float option with get, set
        abstract nullInput: bool option with get, set
        abstract invalidMonth: string option with get, set
        abstract invalidFormat: bool option with get, set
        abstract userInvalidated: bool option with get, set
        abstract iso: bool option with get, set
        abstract parsedDateParts: ResizeArray<obj option> option with get, set
        abstract meridiem: string option with get, set

    type [<AllowNullLiteral>] MomentBuiltinFormat =
        abstract __momentBuiltinFormatBrand: obj option with get, set

    type MomentFormatSpecification =
        U3<string, MomentBuiltinFormat, ResizeArray<U2<string, MomentBuiltinFormat>>>

    [<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module MomentFormatSpecification =
        let ofString v: MomentFormatSpecification = v |> U3.Case1
        let isString (v: MomentFormatSpecification) = match v with U3.Case1 _ -> true | _ -> false
        let asString (v: MomentFormatSpecification) = match v with U3.Case1 o -> Some o | _ -> None
        let ofMomentBuiltinFormat v: MomentFormatSpecification = v |> U3.Case2
        let isMomentBuiltinFormat (v: MomentFormatSpecification) = match v with U3.Case2 _ -> true | _ -> false
        let asMomentBuiltinFormat (v: MomentFormatSpecification) = match v with U3.Case2 o -> Some o | _ -> None
        let ofCase3 v: MomentFormatSpecification = v |> U3.Case3
        let isCase3 (v: MomentFormatSpecification) = match v with U3.Case3 _ -> true | _ -> false
        let asCase3 (v: MomentFormatSpecification) = match v with U3.Case3 o -> Some o | _ -> None

    module UnitOfTime =

        type [<StringEnum>] [<RequireQualifiedAccess>] Base =
            | Year
            | Years
            | Y
            | Month
            | Months
            | [<CompiledName "M">] M
            | Week
            | Weeks
            | W
            | Day
            | Days
            | D
            | Hour
            | Hours
            | H
            | Minute
            | Minutes
            | Second
            | Seconds
            | S
            | Millisecond
            | Milliseconds
            | Ms

        type [<StringEnum>] [<RequireQualifiedAccess>] _quarter =
            | Quarter
            | Quarters
            | [<CompiledName "Q">] Q

        type [<StringEnum>] [<RequireQualifiedAccess>] _isoWeek =
            | IsoWeek
            | IsoWeeks
            | [<CompiledName "W">] W

        type [<StringEnum>] [<RequireQualifiedAccess>] _date =
            | Date
            | Dates
            | [<CompiledName "D">] D

        type DurationConstructor =
            U2<Base, _quarter>

        [<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module DurationConstructor =
            let ofBase v: DurationConstructor = v |> U2.Case1
            let isBase (v: DurationConstructor) = match v with U2.Case1 _ -> true | _ -> false
            let asBase (v: DurationConstructor) = match v with U2.Case1 o -> Some o | _ -> None
            let of_quarter v: DurationConstructor = v |> U2.Case2
            let is_quarter (v: DurationConstructor) = match v with U2.Case2 _ -> true | _ -> false
            let as_quarter (v: DurationConstructor) = match v with U2.Case2 o -> Some o | _ -> None

        type DurationAs =
            Base

        type StartOf =
            U4<Base, _quarter, _isoWeek, _date>

        [<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module StartOf =
            let ofBase v: StartOf = v |> U4.Case1
            let isBase (v: StartOf) = match v with U4.Case1 _ -> true | _ -> false
            let asBase (v: StartOf) = match v with U4.Case1 o -> Some o | _ -> None
            let of_quarter v: StartOf = v |> U4.Case2
            let is_quarter (v: StartOf) = match v with U4.Case2 _ -> true | _ -> false
            let as_quarter (v: StartOf) = match v with U4.Case2 o -> Some o | _ -> None
            let of_isoWeek v: StartOf = v |> U4.Case3
            let is_isoWeek (v: StartOf) = match v with U4.Case3 _ -> true | _ -> false
            let as_isoWeek (v: StartOf) = match v with U4.Case3 o -> Some o | _ -> None
            let of_date v: StartOf = v |> U4.Case4
            let is_date (v: StartOf) = match v with U4.Case4 _ -> true | _ -> false
            let as_date (v: StartOf) = match v with U4.Case4 o -> Some o | _ -> None

        type Diff =
            U2<Base, _quarter>

        [<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Diff =
            let ofBase v: Diff = v |> U2.Case1
            let isBase (v: Diff) = match v with U2.Case1 _ -> true | _ -> false
            let asBase (v: Diff) = match v with U2.Case1 o -> Some o | _ -> None
            let of_quarter v: Diff = v |> U2.Case2
            let is_quarter (v: Diff) = match v with U2.Case2 _ -> true | _ -> false
            let as_quarter (v: Diff) = match v with U2.Case2 o -> Some o | _ -> None

        type MomentConstructor =
            U2<Base, _date>

        [<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module MomentConstructor =
            let ofBase v: MomentConstructor = v |> U2.Case1
            let isBase (v: MomentConstructor) = match v with U2.Case1 _ -> true | _ -> false
            let asBase (v: MomentConstructor) = match v with U2.Case1 o -> Some o | _ -> None
            let of_date v: MomentConstructor = v |> U2.Case2
            let is_date (v: MomentConstructor) = match v with U2.Case2 _ -> true | _ -> false
            let as_date (v: MomentConstructor) = match v with U2.Case2 o -> Some o | _ -> None

        type All =
            obj

    type [<AllowNullLiteral>] MomentInputObject =
        abstract years: float option with get, set
        abstract year: float option with get, set
        abstract y: float option with get, set
        abstract months: float option with get, set
        abstract month: float option with get, set
        abstract M: float option with get, set
        abstract days: float option with get, set
        abstract day: float option with get, set
        abstract d: float option with get, set
        abstract dates: float option with get, set
        abstract date: float option with get, set
        abstract D: float option with get, set
        abstract hours: float option with get, set
        abstract hour: float option with get, set
        abstract h: float option with get, set
        abstract minutes: float option with get, set
        abstract minute: float option with get, set
        abstract m: float option with get, set
        abstract seconds: float option with get, set
        abstract second: float option with get, set
        abstract s: float option with get, set
        abstract milliseconds: float option with get, set
        abstract millisecond: float option with get, set
        abstract ms: float option with get, set

    type [<AllowNullLiteral>] DurationInputObject =
        inherit MomentInputObject
        abstract quarters: float option with get, set
        abstract quarter: float option with get, set
        abstract Q: float option with get, set
        abstract weeks: float option with get, set
        abstract week: float option with get, set
        abstract w: float option with get, set

    type [<AllowNullLiteral>] MomentSetObject =
        inherit MomentInputObject
        abstract weekYears: float option with get, set
        abstract weekYear: float option with get, set
        abstract gg: float option with get, set
        abstract isoWeekYears: float option with get, set
        abstract isoWeekYear: float option with get, set
        abstract GG: float option with get, set
        abstract quarters: float option with get, set
        abstract quarter: float option with get, set
        abstract Q: float option with get, set
        abstract weeks: float option with get, set
        abstract week: float option with get, set
        abstract w: float option with get, set
        abstract isoWeeks: float option with get, set
        abstract isoWeek: float option with get, set
        abstract W: float option with get, set
        abstract dayOfYears: float option with get, set
        abstract dayOfYear: float option with get, set
        abstract DDD: float option with get, set
        abstract weekdays: float option with get, set
        abstract weekday: float option with get, set
        abstract e: float option with get, set
        abstract isoWeekdays: float option with get, set
        abstract isoWeekday: float option with get, set
        abstract E: float option with get, set

    type [<AllowNullLiteral>] FromTo =
        abstract from: MomentInput with get, set
        abstract ``to``: MomentInput with get, set

    type MomentInput =
        U7<Moment, DateTime, string, float, ResizeArray<U2<float, string>>, MomentInputObject, unit>

    [<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module MomentInput =
        let ofMoment v: MomentInput = v |> U7.Case1
        let isMoment (v: MomentInput) = match v with U7.Case1 _ -> true | _ -> false
        let asMoment (v: MomentInput) = match v with U7.Case1 o -> Some o | _ -> None
        let ofDateTime v: MomentInput = v |> U7.Case2
        let isDateTime (v: MomentInput) = match v with U7.Case2 _ -> true | _ -> false
        let asDateTime (v: MomentInput) = match v with U7.Case2 o -> Some o | _ -> None
        let ofString v: MomentInput = v |> U7.Case3
        let isString (v: MomentInput) = match v with U7.Case3 _ -> true | _ -> false
        let asString (v: MomentInput) = match v with U7.Case3 o -> Some o | _ -> None
        let ofFloat v: MomentInput = v |> U7.Case4
        let isFloat (v: MomentInput) = match v with U7.Case4 _ -> true | _ -> false
        let asFloat (v: MomentInput) = match v with U7.Case4 o -> Some o | _ -> None
        let ofCase5 v: MomentInput = v |> U7.Case5
        let isCase5 (v: MomentInput) = match v with U7.Case5 _ -> true | _ -> false
        let asCase5 (v: MomentInput) = match v with U7.Case5 o -> Some o | _ -> None
        let ofMomentInputObject v: MomentInput = v |> U7.Case6
        let isMomentInputObject (v: MomentInput) = match v with U7.Case6 _ -> true | _ -> false
        let asMomentInputObject (v: MomentInput) = match v with U7.Case6 o -> Some o | _ -> None
        let ofUnit v: MomentInput = v |> U7.Case7
        let isUnit (v: MomentInput) = match v with U7.Case7 _ -> true | _ -> false
        let asUnit (v: MomentInput) = match v with U7.Case7 o -> Some o | _ -> None

    type DurationInputArg1 =
        U6<Duration, float, string, FromTo, DurationInputObject, unit>

    [<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module DurationInputArg1 =
        let ofDuration v: DurationInputArg1 = v |> U6.Case1
        let isDuration (v: DurationInputArg1) = match v with U6.Case1 _ -> true | _ -> false
        let asDuration (v: DurationInputArg1) = match v with U6.Case1 o -> Some o | _ -> None
        let ofFloat v: DurationInputArg1 = v |> U6.Case2
        let isFloat (v: DurationInputArg1) = match v with U6.Case2 _ -> true | _ -> false
        let asFloat (v: DurationInputArg1) = match v with U6.Case2 o -> Some o | _ -> None
        let ofString v: DurationInputArg1 = v |> U6.Case3
        let isString (v: DurationInputArg1) = match v with U6.Case3 _ -> true | _ -> false
        let asString (v: DurationInputArg1) = match v with U6.Case3 o -> Some o | _ -> None
        let ofFromTo v: DurationInputArg1 = v |> U6.Case4
        let isFromTo (v: DurationInputArg1) = match v with U6.Case4 _ -> true | _ -> false
        let asFromTo (v: DurationInputArg1) = match v with U6.Case4 o -> Some o | _ -> None
        let ofDurationInputObject v: DurationInputArg1 = v |> U6.Case5
        let isDurationInputObject (v: DurationInputArg1) = match v with U6.Case5 _ -> true | _ -> false
        let asDurationInputObject (v: DurationInputArg1) = match v with U6.Case5 o -> Some o | _ -> None
        let ofUnit v: DurationInputArg1 = v |> U6.Case6
        let isUnit (v: DurationInputArg1) = match v with U6.Case6 _ -> true | _ -> false
        let asUnit (v: DurationInputArg1) = match v with U6.Case6 o -> Some o | _ -> None

    type DurationInputArg2 =
        UnitOfTime.DurationConstructor

    type LocaleSpecifier =
        U5<string, Moment, Duration, ResizeArray<string>, bool>

    [<RequireQualifiedAccess; CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module LocaleSpecifier =
        let ofString v: LocaleSpecifier = v |> U5.Case1
        let isString (v: LocaleSpecifier) = match v with U5.Case1 _ -> true | _ -> false
        let asString (v: LocaleSpecifier) = match v with U5.Case1 o -> Some o | _ -> None
        let ofMoment v: LocaleSpecifier = v |> U5.Case2
        let isMoment (v: LocaleSpecifier) = match v with U5.Case2 _ -> true | _ -> false
        let asMoment (v: LocaleSpecifier) = match v with U5.Case2 o -> Some o | _ -> None
        let ofDuration v: LocaleSpecifier = v |> U5.Case3
        let isDuration (v: LocaleSpecifier) = match v with U5.Case3 _ -> true | _ -> false
        let asDuration (v: LocaleSpecifier) = match v with U5.Case3 o -> Some o | _ -> None
        let ofStringArray v: LocaleSpecifier = v |> U5.Case4
        let isStringArray (v: LocaleSpecifier) = match v with U5.Case4 _ -> true | _ -> false
        let asStringArray (v: LocaleSpecifier) = match v with U5.Case4 o -> Some o | _ -> None
        let ofBool v: LocaleSpecifier = v |> U5.Case5
        let isBool (v: LocaleSpecifier) = match v with U5.Case5 _ -> true | _ -> false
        let asBool (v: LocaleSpecifier) = match v with U5.Case5 o -> Some o | _ -> None

    type [<AllowNullLiteral>] MomentCreationData =
        abstract input: MomentInput with get, set
        abstract format: MomentFormatSpecification option with get, set
        abstract locale: Locale with get, set
        abstract isUTC: bool with get, set
        abstract strict: bool option with get, set

    type [<AllowNullLiteral>] Moment =
        inherit Object
        abstract format: ?format: string -> string
        abstract startOf: unitOfTime: UnitOfTime.StartOf -> Moment
        abstract endOf: unitOfTime: UnitOfTime.StartOf -> Moment
        abstract add: ?amount: DurationInputArg1 * ?unit: DurationInputArg2 -> Moment
        abstract add: unit: UnitOfTime.DurationConstructor * amount: U2<float, string> -> Moment
        abstract subtract: ?amount: DurationInputArg1 * ?unit: DurationInputArg2 -> Moment
        abstract subtract: unit: UnitOfTime.DurationConstructor * amount: U2<float, string> -> Moment
        abstract calendar: ?time: MomentInput * ?formats: CalendarSpec -> string
        abstract clone: unit -> Moment
        abstract valueOf: unit -> float
        abstract local: ?keepLocalTime: bool -> Moment
        abstract isLocal: unit -> bool
        abstract utc: ?keepLocalTime: bool -> Moment
        abstract isUTC: unit -> bool
        abstract isUtc: unit -> bool
        abstract parseZone: unit -> Moment
        abstract isValid: unit -> bool
        abstract invalidAt: unit -> float
        abstract hasAlignedHourOffset: ?other: MomentInput -> bool
        abstract creationData: unit -> MomentCreationData
        abstract parsingFlags: unit -> MomentParsingFlags
        abstract year: y: float -> Moment
        abstract year: unit -> float
        abstract years: y: float -> Moment
        abstract years: unit -> float
        abstract quarter: unit -> float
        abstract quarter: q: float -> Moment
        abstract quarters: unit -> float
        abstract quarters: q: float -> Moment
        abstract month: M: U2<float, string> -> Moment
        abstract month: unit -> float
        abstract months: M: U2<float, string> -> Moment
        abstract months: unit -> float
        abstract day: d: U2<float, string> -> Moment
        abstract day: unit -> float
        abstract days: d: U2<float, string> -> Moment
        abstract days: unit -> float
        abstract date: d: float -> Moment
        abstract date: unit -> float
        abstract dates: d: float -> Moment
        abstract dates: unit -> float
        abstract hour: h: float -> Moment
        abstract hour: unit -> float
        abstract hours: h: float -> Moment
        abstract hours: unit -> float
        abstract minute: m: float -> Moment
        abstract minute: unit -> float
        abstract minutes: m: float -> Moment
        abstract minutes: unit -> float
        abstract second: s: float -> Moment
        abstract second: unit -> float
        abstract seconds: s: float -> Moment
        abstract seconds: unit -> float
        abstract millisecond: ms: float -> Moment
        abstract millisecond: unit -> float
        abstract milliseconds: ms: float -> Moment
        abstract milliseconds: unit -> float
        abstract weekday: unit -> float
        abstract weekday: d: float -> Moment
        abstract isoWeekday: unit -> float
        abstract isoWeekday: d: U2<float, string> -> Moment
        abstract weekYear: unit -> float
        abstract weekYear: d: float -> Moment
        abstract isoWeekYear: unit -> float
        abstract isoWeekYear: d: float -> Moment
        abstract week: unit -> float
        abstract week: d: float -> Moment
        abstract weeks: unit -> float
        abstract weeks: d: float -> Moment
        abstract isoWeek: unit -> float
        abstract isoWeek: d: float -> Moment
        abstract isoWeeks: unit -> float
        abstract isoWeeks: d: float -> Moment
        abstract weeksInYear: unit -> float
        abstract isoWeeksInYear: unit -> float
        abstract dayOfYear: unit -> float
        abstract dayOfYear: d: float -> Moment
        abstract from: inp: MomentInput * ?suffix: bool -> string
        abstract ``to``: inp: MomentInput * ?suffix: bool -> string
        abstract fromNow: ?withoutSuffix: bool -> string
        abstract toNow: ?withoutPrefix: bool -> string
        abstract diff: b: MomentInput * ?unitOfTime: UnitOfTime.Diff * ?precise: bool -> float
        abstract toArray: unit -> ResizeArray<float>
        abstract toDate: unit -> DateTime
        abstract toISOString: ?keepOffset: bool -> string
        abstract inspect: unit -> string
        abstract toJSON: unit -> string
        abstract unix: unit -> float
        abstract isLeapYear: unit -> bool
        abstract zone: unit -> float
        abstract zone: b: U2<float, string> -> Moment
        abstract utcOffset: unit -> float
        abstract utcOffset: b: U2<float, string> * ?keepLocalTime: bool -> Moment
        abstract isUtcOffset: unit -> bool
        abstract daysInMonth: unit -> float
        abstract isDST: unit -> bool
        abstract zoneAbbr: unit -> string
        abstract zoneName: unit -> string
        abstract isBefore: ?inp: MomentInput * ?granularity: UnitOfTime.StartOf -> bool
        abstract isAfter: ?inp: MomentInput * ?granularity: UnitOfTime.StartOf -> bool
        abstract isSame: ?inp: MomentInput * ?granularity: UnitOfTime.StartOf -> bool
        abstract isSameOrAfter: ?inp: MomentInput * ?granularity: UnitOfTime.StartOf -> bool
        abstract isSameOrBefore: ?inp: MomentInput * ?granularity: UnitOfTime.StartOf -> bool
        abstract isBetween: a: MomentInput * b: MomentInput * ?granularity: UnitOfTime.StartOf * ?inclusivity: U4<string, string, string, string> -> bool
        abstract lang: language: LocaleSpecifier -> Moment
        abstract lang: unit -> Locale
        abstract locale: unit -> string
        abstract locale: locale: LocaleSpecifier -> Moment
        abstract localeData: unit -> Locale
        abstract isDSTShifted: unit -> bool
        abstract max: ?inp: MomentInput * ?format: MomentFormatSpecification * ?strict: bool -> Moment
        abstract max: ?inp: MomentInput * ?format: MomentFormatSpecification * ?language: string * ?strict: bool -> Moment
        abstract min: ?inp: MomentInput * ?format: MomentFormatSpecification * ?strict: bool -> Moment
        abstract min: ?inp: MomentInput * ?format: MomentFormatSpecification * ?language: string * ?strict: bool -> Moment
        abstract get: unit: UnitOfTime.All -> float
        abstract set: unit: UnitOfTime.All * value: float -> Moment
        abstract set: objectLiteral: MomentSetObject -> Moment
        abstract toObject: unit -> MomentObjectOutput
