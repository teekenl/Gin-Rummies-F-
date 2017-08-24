module GinRummy

open Cards

// Add helper functions to help compute Deadwood function

let PipValue rankOfCard = 
 // Fixme: an integer value between 1 and 10 
    match rankOfCard.rank with
    | Ace -> 1
    | Two -> 2
    | Three -> 3
    | Four -> 4
    | Five -> 5
    | Six -> 6
    | Seven -> 7
    | Eight -> 8
    | Nine -> 9
    | King | Jack | Queen | Ten -> 10

// Function to check J= 10 , Q = 11, K = 12 in run, eg  9,10,J,Q,K should be a run
// J,J,J should be a set
let PipValue2 rankOfCard = 
    match rankOfCard.rank with
    | Ace -> 1
    | Two -> 2
    | Three -> 3
    | Four -> 4
    | Five -> 5
    | Six -> 6
    | Seven -> 7
    | Eight -> 8
    | Nine -> 9
    | Ten -> 10
    | Jack -> 11
    | Queen -> 12
    | King -> 13

// Return list of card with the same suit
let numberOfSuitInACard cardSuit list = list |> List.choose(fun elem -> 
                                    match elem with 
                                    | elem when elem.suit = cardSuit -> Some(elem)
                                    | _ -> None)   
                                    
// Return number of card with same rank
let numberOfSameRank card list = list |> List.choose(fun elem -> 
                                    match elem with 
                                    | elem when elem.rank = card.rank -> Some(elem)
                                    | _ -> None)  
                                    |> List.length
                                                          
// Sort the number in ascending order for particular group of suit
let BubbleSort (lst : list<int>) = 
    let rec sort accum rev lst =
        match lst, rev with
        | [], true -> accum |> List.rev
        | [], false -> accum |> List.rev |> sort [] true
        | x::y::tail, _ when x > y -> sort (y::accum) false (x::tail)
        | head::tail, _ -> sort (head::accum) rev tail
    sort [] true lst

// Check the sequence from lowest to highest / highest to lowest, and card exists in the sequence , return true it is 
let validSubSeq card list = 
    let sortedList = BubbleSort list
    let cardValue = PipValue2 card
    let first = sortedList.Item(0)

    let rec checkSubSeq pre sub seqList list = 
        match list with 
        | [] -> sub > 2 && seqList |> List.exists (fun elem -> elem = cardValue)
        | head::tail ->
            if pre = head then
                let next = pre + 1
                let subLength = sub + 1
                let newSeqList = List.append seqList [pre] |> Seq.toList
                if (subLength <= 2) then
                     checkSubSeq next subLength newSeqList tail
                else 
                    if newSeqList |> List.exists (fun elem -> elem = cardValue) then
                        checkSubSeq next subLength newSeqList []
                    else 
                        checkSubSeq next subLength newSeqList tail
            else 
                checkSubSeq head 0 [] tail
    
    let validSub = checkSubSeq first 0 [] sortedList
    validSub

// Check the it is a sub gin with a specified card, return true if it is
let CheckSubRunGin card list = 
    let cardSuit = card.suit
    let sameSequenceCardList = numberOfSuitInACard cardSuit list
  
    let sameSequenceCardValue = sameSequenceCardList |> List.map (fun x -> PipValue2 x)
    sameSequenceCardList.Length > 2 && validSubSeq card sameSequenceCardValue

// Add other functions related to Gin Rummy here ...

// Fixme: returns true if and only if card could be used as part of a Run in the given hand 
let InRun (hand: Hand) (card: Card) =   

    let handList = hand |> Seq.toList
    let cardValue = PipValue2 card
    let cardExistInRun = CheckSubRunGin card handList
    cardExistInRun
   
// Fixme: returns true if and only if card could be used as part of a Set in the given hand 
let InSet (hand: Hand) (card: Card) =   
   
    let handList = hand |> Seq.toList    
    let listValue = numberOfSameRank card handList
    listValue > 2

// Return the list of card CAN be used to form set or run
let cardSetOrRunList handList = handList |> List.choose (fun elem ->
                                                match elem with 
                                                | elem when InRun handList elem && InSet handList elem -> Some (elem)
                                                | _ -> None)

// Return the list of card CAN ONLY be used to form set but not run.
let cardOnlySetList handList = handList |> List.choose (fun elem ->
                                                match elem with 
                                                | elem when InSet handList elem && not (InRun handList elem) -> Some(elem)
                                                | _ -> None )

// Return the list of card CAN ONLY be used to form run but not set.
let cardOnlyRunList handList = handList |> List.choose (fun elem ->
                                                match elem with 
                                                | elem when not(InSet handList elem) && InRun handList elem -> Some(elem)
                                                | _ -> None )
// Return the list of card CAN'T be used to form set or run.
let cardNoSetOrRunList handList = handList |> List.choose (fun elem ->
                                                   match elem with 
                                                   | elem when not (InRun handList elem) && not (InSet handList elem) -> Some(elem)
                                                   | _ -> None )
// Function used to determine the remaining list of cards after being used in set
let rec checkSetDeadWood setList sequence handList =
            match handList with
            | [] -> setList
            | head::tail -> 
                   if InSet sequence head then
                        checkSetDeadWood setList sequence tail
                    else
                        let newSetList = List.append setList [head] |> Seq.toList
                        checkSetDeadWood newSetList sequence tail

// Function used to determine the remaining list of cards after being used in run
let rec checkRunDeadWood runList sequence handList = 
    match handList with
    | [] -> runList  
    | head::tail ->
        if InRun sequence head then
            checkRunDeadWood runList sequence tail
        else   
            let newRunList = List.append runList [head] |> Seq.toList

            checkRunDeadWood newRunList sequence tail


// Function used to check the card that can be used in a set or run,
// Use the card to form possible list of card that can only be used in sets.    
// Return remaining list of cards that can't   
let rec removeSet remain sequence list card = 
    match list with
    | [] -> remain
    | head::tail ->  
        let cardValue = PipValue card 
        let headValue = PipValue head
        let notSameCard = not (card.suit = head.suit && card.rank = head.rank)
        if not(InSet sequence head) && notSameCard then
            let newRemain = List.append remain [head] |> Seq.toList
            removeSet newRemain sequence tail card
        else
            removeSet remain sequence tail card

// Function used to pick a card that can be used in a set or run,
// Use that card to form possible list of card that can only be used in runs. 
// Return remaning list of cards that can't be formed   
let rec removeRun remain sequence list card =
    match list with
    | [] -> remain
    | head::tail -> 
        let cardValue = PipValue card
        let notSameCard = not(card.suit = head.suit && card.rank = head.rank)
        if(not(InRun sequence head) && notSameCard) then
            let newRemain = List.append remain [head] |> Seq.toList
            removeRun newRemain sequence tail card
        else
            removeRun remain sequence tail card

// Recursive function to calculate the best possible deadwood score with best possible combination
// of card on the hand
let rec leastDeadWoodScore discard score firstSet secondSet thirdSet fourthSet =
            match firstSet with 
            | [] -> discard
            | head::tail ->
                // Use the first shared card (Set or Run) to form list of
                // cards which can be ONLY be used in set or run
                let newSecondSet = List.append secondSet [head] |> Seq.toList
                let newThirdSet = List.append thirdSet [head] |> Seq.toList 

                // The remaining list of card that can't be used to form set or
                // run with the shared card
                let remainingSet = removeSet [] newSecondSet newSecondSet head
                let remainingRun = removeRun [] newThirdSet newThirdSet head

                // create the new list of remaining card has not been used 
                // along with list of remaining shared card 
                // Two List of remaining card (After Being used SET or RUN)
                let newCardSet = List.append tail remainingSet |> List.append thirdSet |> List.append fourthSet
                let newCardRun = List.append tail remainingRun |> List.append secondSet |> List.append fourthSet

                // Find the possible combination of remaining card (newCardSet)
                // Forming and finding possible set then runs
                let listAfterSet = checkSetDeadWood [] newCardSet newCardSet
                let listAfterSetRun = checkRunDeadWood [] listAfterSet listAfterSet
                
                // Find the possible combination of remaining cards (newCardSet)
                // Forming and finding possible set then runs
                let listAfterRun = checkRunDeadWood [] newCardSet newCardSet 
                let listAfterRunSet = checkSetDeadWood [] listAfterRun listAfterRun

                // The least score after the newCardSet (Forming Set first)
                let totalValueAfterSetRun = 
                        if listAfterSetRun.Length > 0 then 
                            listAfterSetRun |> List.map (fun elem -> PipValue elem) |> List.reduce (fun accum elem -> accum + elem)
                        else
                            0
                // The least score after newCardSet (Forming Run First)
                let totalValueAfterRunSet =
                        if listAfterRunSet.Length > 0 then 
                            listAfterRunSet |> List.map (fun elem -> PipValue elem) |> List.reduce (fun accum elem -> accum + elem)
                        else
                            0

                // The best possible score (newCardSet) between 
                // forming set or run first
                let newCardSetScore = 
                        if totalValueAfterRunSet < totalValueAfterSetRun then 
                            totalValueAfterRunSet
                        else 
                            totalValueAfterSetRun
                // The list of card need to be discarded (newCardSet)
                let newCardSetDiscardList = 
                        if totalValueAfterRunSet < totalValueAfterSetRun then 
                             listAfterRunSet
                         else 
                             listAfterSetRun 
                
                // Find the possible combination of remaining card (newCardRun)
                // Forming and finding possible set then runs
                let listAfterSet02 = checkSetDeadWood [] newCardRun newCardRun
                let listAfterSetRun02 = checkRunDeadWood [] listAfterSet02 listAfterSet02
                
                // Find the possible combination of remaining card (newCardRun)
                // Forming and finding possible run then set
                let listAfterRun02 = checkRunDeadWood [] newCardRun newCardRun 
                let listAfterRunSet02 = checkSetDeadWood [] listAfterRun02 listAfterRun02
                
                // The least score after newCardRun (Forming Set First)
                let totalValueAfterSetRun02 = 
                        if listAfterSetRun02.Length > 0 then 
                             listAfterSetRun02 |> List.map (fun elem -> PipValue elem) |> List.reduce (fun accum elem -> accum + elem)
                        else
                             0
                // The least score after newCardRun (Forming Run First)
                let totalValueAfterRunSet02 =
                        if listAfterRunSet02.Length > 0 then 
                            listAfterRunSet02 |> List.map (fun elem -> PipValue elem) |> List.reduce (fun accum elem -> accum + elem)
                        else
                            0

                // The best possible score (newCardRun) between 
                // forming set or run first
                let newCardRunScore = 
                        if totalValueAfterRunSet02 < totalValueAfterSetRun02 then 
                            totalValueAfterRunSet02
                        else 
                            totalValueAfterSetRun02
                // The list of card need to be discarded ()
                let newCardRunDiscardList = 
                        if totalValueAfterRunSet02 < totalValueAfterSetRun02 then 
                            listAfterRunSet02
                        else 
                            listAfterSetRun02
                 
                // Find the best possible score after the four possible 
                // combination of card has been figured out
                if newCardSetScore < newCardRunScore then 
                    if score  > newCardSetScore then 
                        leastDeadWoodScore newCardSetDiscardList newCardSetScore tail secondSet thirdSet fourthSet
                    else 
                        leastDeadWoodScore discard score tail secondSet thirdSet fourthSet
                else 
                    if score  > newCardRunScore then 
                        leastDeadWoodScore newCardRunDiscardList newCardRunScore tail secondSet thirdSet fourthSet
                    else 
                        leastDeadWoodScore discard score tail secondSet thirdSet fourthSet
            
// Calculate the deadwood score of cards on the hand
let Deadwood (hand:Hand) =
    let handList = hand |> Seq.toList

    // Break the list of card into disjoint set
    let cardSetOrRun = cardSetOrRunList handList
    let cardSet  = cardOnlySetList handList
    let cardRun = cardOnlyRunList handList
    let cardNotRunOrSet = cardNoSetOrRunList handList
    let allSequenceCard = handList |> List.choose (fun elem ->
                                                   match elem with
                                                   | elem when InRun handList elem -> Some(elem)
                                                   | _ -> None) |> List.length

    if cardSetOrRun.Length = 0 || allSequenceCard = handList.Length then
        if cardNotRunOrSet.Length > 0 then 
            cardNotRunOrSet |> List.map (fun elem -> PipValue elem) 
                            |> List.reduce (fun accum elem -> accum + elem)
        else 
            0
    else           
        let leastScore = leastDeadWoodScore [] 100 cardSetOrRun cardSet cardRun cardNotRunOrSet 
                         |> List.map (fun elem -> PipValue elem) 
                         |> List.reduce (fun accum elem -> accum + elem)
        leastScore

// Function to determine the list of card can't be used in set or run, 
// ready to be discarded
let disCardList (hand:Hand) = 
    let handList = hand |> Seq.toList
    
    // Break the list of card into disjoint set
    let cardSetOrRun = cardSetOrRunList handList
    let cardSet  = cardOnlySetList handList
    let cardRun = cardOnlyRunList handList
    let cardNotRunOrSet = cardNoSetOrRunList handList
    let allSequenceCard = handList |> List.choose (fun elem ->
                                                   match elem with
                                                   | elem when InRun handList elem -> Some(elem)
                                                   | _ -> None) |> List.length

    if cardSetOrRun.Length = 0 || allSequenceCard = handList.Length then
        if cardNotRunOrSet.Length > 0 then 
            cardNotRunOrSet 
         else 
            []
    else    
        let rec leastDiscardList = leastDeadWoodScore [] 100 cardSetOrRun cardSet cardRun cardNotRunOrSet
        leastDiscardList 
                
// Fixme change so that it computes how many points should be scored by the firstOut hand
// (score should be negative if the secondOut hand is the winner)    
let Score (firstOut:Hand) (secondOut:Hand) =
    let rulePointFirstOut = 25
    let rulePointSecondOut = -25
    let firstScore = Deadwood firstOut
    let secondScore = Deadwood secondOut
   
    if firstScore < secondScore then 
        if firstScore = 0 then
            let totalValue = rulePointFirstOut + secondScore
            totalValue
        else 
            secondScore - firstScore         
    elif firstScore = secondScore then
         if firstScore = 0 then
            rulePointFirstOut
         else
            rulePointSecondOut
    else
        let difference = secondScore - firstScore
        rulePointSecondOut + difference
