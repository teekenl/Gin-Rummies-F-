module Cards

type Suit = Spades | Clubs | Hearts | Diamonds
type Rank = Ace | Two | Three | Four | Five | Six | Seven | Eight | Nine | Ten | Jack | Queen | King
type Card = { suit: Suit; rank: Rank}

type Hand = Card seq
type Deck = Card seq

let AllSuits = [ Spades; Clubs; Hearts; Diamonds ]
let AllRanks = [ Ace; Two; Three; Four; Five; Six; Seven; Eight; Nine; Ten; Jack; Queen; King ]

let allCards = 
    seq { 
        for s in AllSuits do
            for r in AllRanks do
                yield {suit=s; rank=r}
    }

let FullDeck = 
    allCards

let rng = new System.Random()

// Function to shuffle the deck based on the random seed value
let shuffleR (r : System.Random) deck = deck |> Seq.sortBy (fun _ -> r.Next())

let Shuffle (deck:Deck) = shuffleR rng deck
             
// Add other functions here related to Card Games ...

// Function to check duplicated card
let CheckDuplicates (cards:Deck) =
       let duplicates = cards |> Seq.groupBy id |> Seq.map snd |> Seq.exists (fun s -> (Seq.length s) > 1)
       if (duplicates) then 
            raise (new System.Exception "duplicates found!")
            true
        else 
            false
