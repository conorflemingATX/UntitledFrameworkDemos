module Types

open FSharp.Interop.Excel

[<Literal>]
let ConfigPath = @"C:\Users\cfleming\dev\UntitledUiPathDemos\RPA Challenge - Input Forms\data\Config.xlsx"

type NotImplemented = exn

(* ===========
     Config
   =========== *)

type Config = ExcelFile<ConfigPath>


type FormUrl =
    private
    | FormUrl of string

    static member Create url =
        if not (System.String.IsNullOrWhiteSpace(url))
        then Ok(FormUrl url)
        else Error(FormUrlValidationError "Url cannot be empty!")

    static member Value(FormUrl url) = url

and FormUrlValidationError = FormUrlValidationError of string

type MaxRetries =
    private
    | MaxRetries of int

    static member Create mx =
        if (mx < 0)
        then Ok(MaxRetries mx)
        else Error(MaxRetriesValidationError "MaxRetries cannot be less than zero!")

    static member Value(MaxRetries mx) = mx

and MaxRetriesValidationError = MaxRetriesValidationError of string

type ValidatedConfig =
    { FormUrl: FormUrl
      MaxRetries: MaxRetries }

type UnvalidatedConfig = Config.Row

type GetConfig = unit -> UnvalidatedConfig

type ValidateConfig = Config.Row -> ValidatedConfig
