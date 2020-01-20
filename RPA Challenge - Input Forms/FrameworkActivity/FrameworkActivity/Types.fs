module Types

open System
open FSharp.Interop.Excel

[<Literal>]
let ConfigPath = @"C:\Users\cfleming\dev\UntitledUiPathDemos\RPA Challenge - Input Forms\data\Config.xlsx"

[<Literal>]
let ChallengePath = @"C:\Users\cfleming\dev\UntitledUiPathDemos\RPA Challenge - Input Forms\data\Challenge.xlsx"

type Undefined = exn

type Validation<'a, 'b> =
    | Success of 'a
    | Failure of 'b list // Usually this would be string, but make it generic to match Result

    member x.Map f xResult =
        match xResult with
        | Success x -> Success(f x)
        | Failure errs -> Failure errs

    member x.Point x' = Success x'

    member x.Apply fResult xResult =
        match fResult, xResult with
        | Success f, Success x' -> Success(f x')
        | Failure errs, Success _ -> Failure errs
        | Success f, Failure errs -> Failure errs
        | Failure errs1, Failure errs2 -> Failure(List.concat [ errs1; errs2 ])

    member x.Bind f xResult =
        match xResult with
        | Success x' -> f x'
        | Failure errs -> Failure errs

    member x.FromResult =
        function
        | Ok x' -> Success x'
        | Error x' -> Failure [ x' ]

(* ===========
     Config
   =========== *)

type FormUrl =
    private
    | FormUrl of string

    static member Create(url: string) =
        if String.IsNullOrWhiteSpace(url) then Error "Url is required." else Ok(FormUrl url)

    static member Value(FormUrl url) = url

type MaxRetries =
    private
    | MaxRetries of int

    static member Create(i: int) =
        if i < 0
        then Error "Max retries cannot be less than 0."
        else Ok(MaxRetries i)

    static member Value(MaxRetries i) = i

type Config = ExcelFile<ConfigPath>

type UnvalidatedConfig = Config.Row

type ValidatedConfig =
    { FormUrl: FormUrl
      MaxRetries: MaxRetries }

(* ===========
     Challenge Data
   =========== *)

type FormId = FormId of int

type FirstName =
    private
    | FirstName of string

    static member Create(name: string) =
        if String.IsNullOrWhiteSpace(name) then Error "First name is required." else Ok(FirstName name)

    static member Value(FirstName name) = name

type LastName =
    private
    | LastName of string

    static member Create(name: string) =
        if String.IsNullOrWhiteSpace(name) then Error "Last name is required." else Ok(LastName name)

    static member Value(LastName name) = name

type CompanyName =
    private
    | CompanyName of string

    static member Create(company: string) =
        if String.IsNullOrWhiteSpace(company) then Error "Company name is required." else Ok(CompanyName company)

    static member Value(CompanyName company) = company

type Role =
    private
    | Role of string

    static member Create(role: string) =
        if String.IsNullOrWhiteSpace(role) then Error "Role is required." else Ok(Role role)

    static member Value(Role role) = role

type Address =
    private
    | Address of string

    static member Create(add: string) =
        if String.IsNullOrWhiteSpace(add) then Error "Address is required." else Ok(Address add)

    static member Value(Address add) = add

type Email =
    private
    | Email of string

    static member Create(email: string) =
        if String.IsNullOrWhiteSpace(email) then Error "Email is required." else Ok(Email email)

    static member Value(Email email) = email

type PhoneNumber =
    private
    | PhoneNumber of string

    static member Create(num: string) =
        if String.IsNullOrWhiteSpace(num) then Error "PhoneNumber is required." else Ok(PhoneNumber num)

    static member Value(PhoneNumber num) = num

type Challenge = ExcelFile<ChallengePath>

type UnvalidatedFormInput = Challenge.Row

type InvalidFormInput =
    { Id: int
      FirstName: FirstName option
      LastName: LastName option
      CompanyName: CompanyName option
      Role: Role option
      Address: Address option
      Email: Email option
      PhoneNumber: PhoneNumber option
      Error: string list }

type ValidatedFormInput =
    { Id: int
      FirstName: FirstName
      LastName: LastName
      CompanyName: CompanyName
      Role: Role
      Address: Address
      Email: Email
      PhoneNumber: PhoneNumber }

type SuccessfulFormInput =
    { Id: int
      FirstName: FirstName
      LastName: LastName
      CompanyName: CompanyName
      Role: Role
      Address: Address
      Email: Email
      PhoneNumber: PhoneNumber
      TimeStamp: string }

type UnsuccessfulFormInput =
    { Id: int
      FirstName: FirstName
      LastName: LastName
      CompanyName: CompanyName
      Role: Role
      Address: Address
      Email: Email
      PhoneNumber: PhoneNumber
      TimeStamp: string
      Error: string }

type FormInputRoot =
    { InvalidFormInputs: InvalidFormInput list
      ValidatedFormInputs: ValidatedFormInput list
      SuccessfulFormInputs: SuccessfulFormInput list
      UnsuccessfulFormInputs: UnsuccessfulFormInput list }

(* ===========
    Process Errors
   =========== *)

type Timestamp = Timestamp of DateTime

type ErrorMessage =
    private
    | Message of string

    static member Create(msg: string) =
        if String.IsNullOrWhiteSpace(msg) then Error "Error Message cannot be empty." else Ok(Message msg)

    static member Value(Message msg) = msg

type Err =
    { Timestamp: Timestamp
      Message: ErrorMessage }

type ApplicationError =
    | ConfigNotFoundError of Err
    | InvalidConfigError of Err
    | FormInputNotFoundError of Err
    | NoValidFormInputs of Err
    | PageNotFoundError of Err
    | CleanUpResourcesError of Err

(* ===========
     State
   =========== *)

type ReadingFile =
    { Config: ValidatedConfig }

type NavigatingToSite =
    { Config: ValidatedConfig
      FormInput: FormInputRoot }

type InputtingFormData =
    { Config: ValidatedConfig
      FormInput: FormInputRoot }

type ClosingOpenResources =
    { Config: ValidatedConfig
      FormInput: FormInputRoot }

type ExitingOnSuccess =
    { Config: ValidatedConfig
      FormInput: FormInputRoot }

type ExitingOnError =
    { Config: ValidatedConfig
      FormInput: FormInputRoot
      Error: ApplicationError }

type State =
    | Init
    | ReadingFile of ReadingFile
    | NavigatingToSite of NavigatingToSite
    | InputtingFormData of InputtingFormData
    | ClosingOpenResources of ClosingOpenResources
    | ExitingOnSuccess of ExitingOnSuccess
    | ExitingOnError of ExitingOnError

(* ===========
    Messages
   =========== *)

type Message =
    | ConfigOk of ValidatedConfig
    | ConfigFailure of ApplicationError
    | ReadingFileOk of FormInputRoot
    | ReadingFileFailure of ApplicationError
    | NavigatingToSiteOk
    | NavigatingToSiteFailure of ApplicationError
    | FormUpdateOk of FormId
    | FormUpdateFailure of FormId
    | ClosingOpenResourcesOk
    | ClosingOpenResourcesFailure of ApplicationError
    | ExitingOnSuccessFailure of ApplicationError
    | ExitingOnErrorFailure of ApplicationError

(* ===========
     Commands
   =========== *)

type Commands =
    | LogError of ApplicationError
    | LogInfo of string
    | NavigateToPage of FormUrl
    | InputToForm of ValidatedFormInput
    | CloseWindow
    | ExitOnSuccess
    | ExitOnError of ApplicationError

(* ===========
    DTOs
   =========== *)

// * One important thing to note is that I'm not sure that I will actually have to serialize the state or not.
// * I'm doing it here to practice, but it may be that options and F# types are fine so long as they are not accessed or used by the UiPath side code.
// * They will be protected from manipulation, so it might be fine.

type ErrDTO =
    ValueTuple<DateTime, string> // Not only held in state will have to be generated in vb which is by far the most cumbersome.

type ApplicationErrorDTO = ValueTuple<string, ErrDTO> // same, hence the valuetuple.

type StateDTO = string

type MessageDTO = ValueTuple<string, obj> // Probably not the best but ok for now.

type CommandDTO = ValueTuple<string, obj> // same

