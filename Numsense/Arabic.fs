﻿module internal Ploeh.Numsense.Arabic

open Ploeh.Numsense.InternalDsl

let rec internal toAraibcImp x =

    // Esentially simplifies expressions like 'twenty-zero' to 'twenty',
    // 'one-milion-zero' to 'one-million', and so on.
    let simplify prefix factor x =
        let remainder = x % factor
        if remainder = 0
        then prefix
        else sprintf "%s-%s" prefix (toAraibcImp (remainder))

    let format suffix factor x =
        let prefix = sprintf "%s%s" (toAraibcImp (x / factor)) suffix
        simplify prefix factor x

    match x with
    |  x when x < 0 -> sprintf "minus %s" (toAraibcImp -x)
    |  0 -> "zero"
    |  1 -> "one"
    |  2 -> "two"
    |  3 -> "three"
    |  4 -> "four"
    |  5 -> "five"
    |  6 -> "six"
    |  7 -> "seven"
    |  8 -> "eight"
    |  9 -> "nine"
    | 10 -> "ten"
    | 11 -> "eleven"
    | 12 -> "twelve"
    | 13 -> "thirteen"
    | 14 -> "fourteen"
    | 15 -> "fifteen"
    | 16 -> "sixteen"
    | 17 -> "seventeen"
    | 18 -> "eighteen"
    | 19 -> "nineteen"
    | Between 20 30 x -> simplify "twenty" 10 x
    | Between 30 40 x -> simplify "thirty" 10 x
    | Between 40 50 x -> simplify "forty" 10 x
    | Between 50 60 x -> simplify "fifty" 10 x
    | Between 80 90 x -> simplify "eighty" 10 x
    | Between 60 100 x -> format "ty" 10 x
    | Between 100 1000 x -> format "-hundred" 100 x
    | Between 1000 1000000 x -> format "-thousand" 1000 x
    | Between 1000000 1000000000 x -> format "-million" 1000000 x
    | _ -> format "-billion" 1000000000 x

let internal tryParseEnglishImp (x : string) =
    let rec conv acc candidate =
        match candidate with
        | ""                      -> Some acc
        | StartsWith "-"        t
        | StartsWith "AND"      t -> conv                acc  t
        | StartsWith "ZERO"     t -> conv          (0  + acc) t
        | StartsWith "ONE"      t -> conv          (1  + acc) t
        | StartsWith "TWO"      t -> conv          (2  + acc) t
        | StartsWith "THREE"    t -> conv          (3  + acc) t
        | StartsWith "FOUR"     t -> conv          (4  + acc) t
        | StartsWith "FIVE"     t -> conv          (5  + acc) t
        | StartsWith "SIX"      t -> conv          (6  + acc) t
        | StartsWith "SEVEN"    t -> conv          (7  + acc) t
        | StartsWith "EIGHT"    t -> conv          (8  + acc) t
        | StartsWith "NINE"     t -> conv          (9  + acc) t
        | StartsWith "TEN"      t -> conv         (10  + acc) t
        | StartsWith "ELEVEN"   t -> conv         (11  + acc) t
        | StartsWith "TWELVE"   t -> conv         (12  + acc) t
        | StartsWith "THIRTEEN" t -> conv         (13  + acc) t
        | StartsWith "FIFTEEN"  t -> conv         (15  + acc) t
        | StartsWith "EEN"      t // matches 'een' in 'eighteen'
        | StartsWith "TEEN"     t -> conv         (10  + acc) t
        | StartsWith "TWENTY"   t -> conv         (20  + acc) t
        | StartsWith "THIRTY"   t -> conv         (30  + acc) t
        | StartsWith "FORTY"    t -> conv         (40  + acc) t
        | StartsWith "FIFTY"    t -> conv         (50  + acc) t
        | StartsWith "Y"        t // matches 'y' in 'eighty'
        | StartsWith "TY"       t -> conv         (10 %* acc) t
        | StartsWith "HUNDRED"  t ->
            conv (if acc = 0 then  100 else       100 %* acc) t
        | StartsWith "THOUSAND" t ->
            conv (if acc = 0 then 1000 else      1000 %* acc) t
        | StartsWith "MILLION"  t -> conv    (1000000 %* acc) t
        | StartsWith "BILLION"  t -> conv (1000000000  * acc) t
        | _ -> None

    let canonicalized = x.Trim().ToUpper(System.Globalization.CultureInfo "en")
    match canonicalized with
    | StartsWith "MINUS" t -> conv 0 (t.Trim ()) |> Option.map ((*)-1)
    | _ -> conv 0 canonicalized