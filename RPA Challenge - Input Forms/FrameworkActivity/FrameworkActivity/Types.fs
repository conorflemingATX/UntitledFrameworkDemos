module Types

(* 
    I have been reading up from "Domain Modelling Made Functional" by Scott Wlaschin, to see if I can't make better sense of this and to an extent I think I have some pretty good ideas of how this is supposed to go. Especially, now that I know the general pattern for what I am doing with having a stateless process be rehydrated everytime is called a "Saga", and the fact that I can have the state be a discrimiated union with cases for every stage of the process each containing different datatypes modelling the state at every point of the process.

    I am also thinking about how to use contrained types properly for use in validation.

    What I really need to do is to simply sit down, and really try to get to grips with everything, but so far that kind of dedication has eluded me.
*)