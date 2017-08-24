module ComputerPlayer

open Cards
open GinRummy

type Move = Gin | Knock | Continue

let ComputerPickupDiscard computerHand topDiscard possibleDeck =
    // Fixme: change function so that it computes if Computer should pickup from Discard pile 
    //        or draw a fresh card from the deck
    let discardDeadwoodScore = Seq.append computerHand (Seq.singleton topDiscard) |> Deadwood
    let averagePossibleScore = possibleDeck 
                                |> Seq.map (fun elem -> (Seq.append computerHand (Seq.singleton elem)) |> Deadwood)
                                |> Seq.averageBy (fun elem -> float elem)
    
    float discardDeadwoodScore < averagePossibleScore 


let ComputerMove newHand =
    
    // Fixme: change function so that it computes which action the Computer should take: Continue, Knock or Gin 
    //        and which card would be best to discard
    let initialScore = Deadwood newHand

    if initialScore = 0 then
        (Gin, None)
    else 
        let listOfDiscard = disCardList newHand
        let selectedDiscardValue = listOfDiscard |> List.map (fun elem -> PipValue elem) |> List.max
        let selectedDiscardIndex = listOfDiscard |> List.map(fun elem -> PipValue elem) |> List.findIndex (fun elem -> elem = selectedDiscardValue)
        let selectedDiscard = List.nth listOfDiscard selectedDiscardIndex
        let discardListScore = listOfDiscard |> List.map (fun elem -> PipValue elem) |> List.reduce (fun accum elem -> accum + elem)

        let nextMoveScore = discardListScore - selectedDiscardValue

        if nextMoveScore > 0 && nextMoveScore <= 10 then
            (Knock, Some selectedDiscard)
        elif nextMoveScore = 0 then
            (Gin, Some selectedDiscard)
        else
            (Continue, Some selectedDiscard)

