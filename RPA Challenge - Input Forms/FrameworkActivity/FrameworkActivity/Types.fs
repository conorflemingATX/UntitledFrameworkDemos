module Types

(* 
    What are the stages that the process goes through?

    1. Init
    2. Read Excel file
    3. Validate Excel data
    4. Send to input form

*)

type NotImplemented = exn

// Doesn't really need a config, but I'm adding one for future reference.
type Config = NotImplemented

type UnvalidatedExcelState = NotImplemented

type ValidatedExcelState = NotImplemented

type UpdateInfo = NotImplemented

(* 
    I'm honestly not 100% about doing it like this. I know that I want a state machine structure like in 'DDD Made Functional which is similar to this, but having these different stages like this AND completely different data structures might be redundant?

    I have split the config out into a pair with assoc. state, considering this will have to be serialized, is that a good idea?
 *)
type State =
    | Init
    | ReadingExcel of Config
    | ValidatingExcel of Config * UnvalidatedExcelState
    | UpdatingInput of Config * ValidatedExcelState
    | UpdateComplete of Config * UpdateInfo

