module Types

open FSharp.Interop.Excel

[<Literal>]
let ConfigPath = @"C:\Users\cfleming\dev\UntitledUiPathDemos\RPA Challenge - Input Forms\data\Config.xlsx"

[<Literal>]
let ChallengePath = @"C:\Users\cfleming\dev\UntitledUiPathDemos\RPA Challenge - Input Forms\data\Challenge.xlsx"

type NotImplemented = exn

(* ===========
     Config
   =========== *)

type Config = ExcelFile<ConfigPath>

type UnvalidatedConfig = Config.Row

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

type ValidateConfig = Config.Row -> ValidatedConfig

type GetConfig = unit -> UnvalidatedConfig


(* ===========
     Challenge Data
   =========== *)

// TODO: define the create and validate functions on the constrained simple types, and think about how to handle inputs that fail validation.

type Challenge = ExcelFile<ChallengePath>

type UnvalidatedFormInput = Challenge.Row

type FirstName = Undefined

and FirstNameError = FirstNameError of string

type LastName = Undefined

and LastNameError = LastNameError of string

type CompanyName = Undefined

and CompanyNameError = CompanyNameError of string

type Role = Undefined

and RoleError = RoleError of string

type Address = Undefined

and AddressError = AddressError of string

type Email = Undefined

and EmailError = EmailError of string

type PhoneNumber = Undefined

and PhoneNumberError = PhoneNumberError of string

type FormInputValidationError =
    | FirstNameError
    | LastNameError
    | CompanyNameError
    | RoleError
    | AddressError
    | EmailError
    | PhoneNumberError

type ValidatedFormInput =
    { FirstName: FirstName
      LastName: LastName
      CompanyName: CompanyName
      Role: Role
      Address: Address
      Email: Email
      PhoneNumber: PhoneNumber }

type InvalidFormInput =
    { RowNumber: int
      Errors: FormUrlValidationError list }

(* ===========
     State
   =========== *)

type Init = Init

type Configured =
    { Config: ValidatedConfig }

type ReadFile =
    { Config: ValidatedConfig
      FormInput: ValidatedFormInput list
      InvalidFormInput: InvalidFormInput list }
// What are the steps of for the rest of process?
