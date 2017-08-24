module UnitTestProject1

open TestData
open Microsoft.VisualStudio.TestTools.UnitTesting


[<TestClass>]
type ShuffleTest() = 
    [<TestMethod>] 
    member x.ShuffleTest01() =
        let shuffled = Cards.Shuffle(Cards.FullDeck) 
        let duplicates = Cards.CheckDuplicates shuffled
        Assert.AreEqual(false,duplicates);
        Assert.AreEqual(Seq.length Cards.FullDeck, Seq.length shuffled)
    [<TestMethod>] 
    member x.ShuffleQualityTest02() =
        let ShuffleOnce shuffle history iteration =
            let shuffled = (shuffle Cards.FullDeck) |> Seq.toList
            let tally (old_tally:Map<Cards.Card*Cards.Card,int>) i = 
                let pair = (shuffled.[i],shuffled.[i+1])
                let old_total = old_tally.[pair]
                let new_tally = Map.add pair (old_total+1) old_tally
                new_tally
            List.fold tally history [0..50]
        let ShuffleQuality shuffle =
            let E = 100 // repeat 100 tmes
            let allPairs = seq { for c1 in Cards.FullDeck do for c2 in Cards.FullDeck do if (c1 <> c2) then yield (c1,c2) }
            let initialTally = Seq.fold (fun tally pair -> (Map.add pair 0 tally)) (Map.empty) allPairs
            let pairs = List.fold (ShuffleOnce shuffle) initialTally [1..(52*E)] |> Map.toSeq 
            let variations = pairs |> Seq.map (fun (pair,freq) -> double ((freq-E)*(freq-E))) 
            let standard_deviation = sqrt (Seq.average variations)
            standard_deviation
        let quality = ShuffleQuality Cards.Shuffle
        Assert.IsTrue(quality < 20.0) // ideally quality should be approximately 10

[<TestClass>]
type DeadwoodTestsSimple() = 
    [<TestMethod>] member x.Deadwood98()    = Assert.AreEqual(98, GinRummy.Deadwood Hand98)
    [<TestMethod>] member x.Deadwood55()    = Assert.AreEqual(55, GinRummy.Deadwood Hand55)
    [<TestMethod>] member x.Deadwood49()    = Assert.AreEqual(49, GinRummy.Deadwood Hand49)
    [<TestMethod>] member x.Deadwood45()    = Assert.AreEqual(45, GinRummy.Deadwood Hand45)
    [<TestMethod>] member x.Deadwood40()    = Assert.AreEqual(40, GinRummy.Deadwood Hand40)
    [<TestMethod>] member x.Deadwood33()    = Assert.AreEqual(33, GinRummy.Deadwood Hand33)
    [<TestMethod>] member x.Deadwood36()    = Assert.AreEqual(36, GinRummy.Deadwood Hand36)
    [<TestMethod>] member x.Deadwood27()    = Assert.AreEqual(27, GinRummy.Deadwood Hand27)
    [<TestMethod>] member x.Deadwood19()    = Assert.AreEqual(19, GinRummy.Deadwood Hand19)
    [<TestMethod>] member x.Deadwood10()    = Assert.AreEqual(10, GinRummy.Deadwood Hand10)
    [<TestMethod>] member x.Deadwood001()   = Assert.AreEqual(0,  GinRummy.Deadwood Gin01)
    [<TestMethod>] member x.Deadwood50()    = Assert.AreEqual(50, GinRummy.Deadwood Hand50)
    [<TestMethod>] member x.Deadwood47()    = Assert.AreEqual(46, GinRummy.Deadwood Hand46)
    [<TestMethod>] member x.Deadwood42()    = Assert.AreEqual(42, GinRummy.Deadwood Hand42)
    [<TestMethod>] member x.Deadwood32()    = Assert.AreEqual(32, GinRummy.Deadwood Hand32)
    [<TestMethod>] member x.Deadwood002()   = Assert.AreEqual(0,  GinRummy.Deadwood Gin02)
    [<TestMethod>] member x.Deadwood35()    = Assert.AreEqual(34, GinRummy.Deadwood Hand34)
   
[<TestClass>]
type DeadwoodTestsDifficult() = 
    [<TestMethod>] member x.Deadwood43()    = Assert.AreEqual(43, GinRummy.Deadwood Hand43)
    [<TestMethod>] member x.Deadwood30()    = Assert.AreEqual(30, GinRummy.Deadwood Hand30)
    [<TestMethod>] member x.Deadwood29()    = Assert.AreEqual(29, GinRummy.Deadwood Hand29)  
    [<TestMethod>] member x.Deadwood21()    = Assert.AreEqual(21, GinRummy.Deadwood Hand21)
    [<TestMethod>] member x.Deadwood14()    = Assert.AreEqual(14, GinRummy.Deadwood Hand14)
    [<TestMethod>] member x.Deadwood22()    = Assert.AreEqual(22, GinRummy.Deadwood Hand22)
    [<TestMethod>] member x.Deadwood01()    = Assert.AreEqual(1,  GinRummy.Deadwood Hand01)
    [<TestMethod>] member x.Deadwood06()    = Assert.AreEqual(6,  GinRummy.Deadwood Hand06)
    [<TestMethod>] member x.Deadwood003()   = Assert.AreEqual(0,  GinRummy.Deadwood Gin03)


[<TestClass>]
type ScoreTest() = 
    [<TestMethod>] member x.Score01()   = Assert.AreEqual(26,  GinRummy.Score Gin01  Hand01)
    [<TestMethod>] member x.Score02()   = Assert.AreEqual(25,  GinRummy.Score Gin01  Gin02)
    [<TestMethod>] member x.Score03()   = Assert.AreEqual(31,  GinRummy.Score Gin01  Hand06)
    [<TestMethod>] member x.Score04()   = Assert.AreEqual(35,  GinRummy.Score Gin01  Hand10)
    [<TestMethod>] member x.Score05()   = Assert.AreEqual(39,  GinRummy.Score Gin01  Hand14)
    [<TestMethod>] member x.Score06()   = Assert.AreEqual(80,  GinRummy.Score Gin01  Hand55)
    [<TestMethod>] member x.Score07()   = Assert.AreEqual(-25,  GinRummy.Score Hand01 Hand01)
    [<TestMethod>] member x.Score08()   = Assert.AreEqual(-26,  GinRummy.Score Hand01 Gin02)
    [<TestMethod>] member x.Score09()   = Assert.AreEqual(5,  GinRummy.Score Hand01 Hand06)
    [<TestMethod>] member x.Score10()   = Assert.AreEqual(9,  GinRummy.Score Hand01 Hand10)
    [<TestMethod>] member x.Score11()   = Assert.AreEqual(13,  GinRummy.Score Hand01 Hand14)
    [<TestMethod>] member x.Score12()   = Assert.AreEqual(54,  GinRummy.Score Hand01 Hand55)
    [<TestMethod>] member x.Score13()   = Assert.AreEqual(-30,  GinRummy.Score Hand06 Hand01)
    [<TestMethod>] member x.Score14()   = Assert.AreEqual(-31,  GinRummy.Score Hand06 Gin02)
    [<TestMethod>] member x.Score15()   = Assert.AreEqual(-25,  GinRummy.Score Hand06 Hand06)
    [<TestMethod>] member x.Score16()   = Assert.AreEqual(4,  GinRummy.Score Hand06 Hand10)
    [<TestMethod>] member x.Score17()   = Assert.AreEqual(8,  GinRummy.Score Hand06 Hand14)
    [<TestMethod>] member x.Score18()   = Assert.AreEqual(49,  GinRummy.Score Hand06 Hand55)
    [<TestMethod>] member x.Score19()   = Assert.AreEqual(-34,  GinRummy.Score Hand10 Hand01)
    [<TestMethod>] member x.Score20()   = Assert.AreEqual(-35,  GinRummy.Score Hand10 Gin02)
    [<TestMethod>] member x.Score21()   = Assert.AreEqual(-29,  GinRummy.Score Hand10 Hand06)
    [<TestMethod>] member x.Score22()   = Assert.AreEqual(-25,  GinRummy.Score Hand10 Hand10)
    [<TestMethod>] member x.Score23()   = Assert.AreEqual(4,  GinRummy.Score Hand10 Hand14)
    [<TestMethod>] member x.Score24()   = Assert.AreEqual(45,  GinRummy.Score Hand10 Hand55)

[<TestClass>]
type PickupTest() = 
    [<TestMethod>] member x.Pickup01() = Assert.IsTrue(ComputerPlayer.ComputerPickupDiscard Hand36 S3 Deck01)
    [<TestMethod>] member x.Pickup02() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand36 S5 Deck01)
    [<TestMethod>] member x.Pickup03() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand36 S9 Deck01)
    [<TestMethod>] member x.Pickup04() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand36 S10 Deck01)
    [<TestMethod>] member x.Pickup05() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand36 SQ Deck01)
    [<TestMethod>] member x.Pickup06() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand36 SK Deck01)
    [<TestMethod>] member x.Pickup07() = Assert.IsTrue(ComputerPlayer.ComputerPickupDiscard Hand36 C3 Deck01)
    [<TestMethod>] member x.Pickup08() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand36 C4 Deck01)
    [<TestMethod>] member x.Pickup09() = Assert.IsTrue(ComputerPlayer.ComputerPickupDiscard Hand36 C7 Deck01)
    [<TestMethod>] member x.Pickup10() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand36 C9 Deck01)
    [<TestMethod>] member x.Pickup11() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand36 C10 Deck01)
    [<TestMethod>] member x.Pickup12() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand36 CK Deck01)
    [<TestMethod>] member x.Pickup13() = Assert.IsTrue(ComputerPlayer.ComputerPickupDiscard Hand36 HA Deck01)
    [<TestMethod>] member x.Pickup14() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand36 H4 Deck01)
    [<TestMethod>] member x.Pickup15() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand36 H5 Deck01)
    [<TestMethod>] member x.Pickup16() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand36 H7 Deck01)
    [<TestMethod>] member x.Pickup17() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand36 HK Deck01)
    [<TestMethod>] member x.Pickup18() = Assert.IsTrue(ComputerPlayer.ComputerPickupDiscard Hand36 DA Deck01)
    [<TestMethod>] member x.Pickup19() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand36 D4 Deck01)
    [<TestMethod>] member x.Pickup20() = Assert.IsTrue(ComputerPlayer.ComputerPickupDiscard Hand36 D6 Deck01)
    [<TestMethod>] member x.Pickup21() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand36 D9 Deck01)
    [<TestMethod>] member x.Pickup22() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand36 D10 Deck01)
    [<TestMethod>] member x.Pickup23() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand98 D7 Deck02)
    [<TestMethod>] member x.Pickup24() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand98 H7 Deck02)
    [<TestMethod>] member x.Pickup25() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand98 D8 Deck02)
    [<TestMethod>] member x.Pickup26() = Assert.IsTrue(ComputerPlayer.ComputerPickupDiscard Hand98 C8 Deck02)
    [<TestMethod>] member x.Pickup27() = Assert.IsTrue(ComputerPlayer.ComputerPickupDiscard Hand98 S8 Deck02)
    [<TestMethod>] member x.Pickup28() = Assert.IsFalse(ComputerPlayer.ComputerPickupDiscard Hand98 H8 Deck02)
    [<TestMethod>] member x.Pickup29() = Assert.IsTrue(ComputerPlayer.ComputerPickupDiscard Hand98 H9 Deck02)
    [<TestMethod>] member x.Pickup30() = Assert.IsTrue(ComputerPlayer.ComputerPickupDiscard Hand98 HK Deck02)


[<TestClass>]
type DiscardTest() = 
    [<TestMethod>] 
    member x.Discard01() = 
        let (move,discard) = ComputerPlayer.ComputerMove Hand64
        Assert.AreEqual(ComputerPlayer.Continue, move)
        Assert.AreEqual(Some H10, discard)
    [<TestMethod>] 
    member x.Discard02() = 
        let (move,discard) = ComputerPlayer.ComputerMove Hand37
        Assert.AreEqual(ComputerPlayer.Continue, move)
        Assert.AreEqual(Some C8, discard)
    [<TestMethod>] 
    member x.Discard03() = 
        let (move,discard) = ComputerPlayer.ComputerMove Hand20
        Assert.AreEqual(ComputerPlayer.Continue, move)
        Assert.AreEqual(Some D8, discard)
    [<TestMethod>] 
    member x.Discard04() = 
        let (move,discard) = ComputerPlayer.ComputerMove Hand18
        Assert.AreEqual(ComputerPlayer.Knock, move)
        Assert.AreEqual(Some C9, discard)
    [<TestMethod>] 
    member x.Discard05() = 
        let (move,discard) = ComputerPlayer.ComputerMove Hand02
        Assert.AreEqual(ComputerPlayer.Gin, move)
        Assert.AreEqual(Some S2, discard)
    [<TestMethod>] 
    member x.Discard06() = 
        let (move,discard) = ComputerPlayer.ComputerMove Gin04
        Assert.AreEqual(ComputerPlayer.Gin, move)
        Assert.AreEqual(None, discard)

