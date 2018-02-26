namespace Solar

module Xml =
    open System
    open System.IO
    open System.Xml
    open System.Drawing
    open System.Globalization

    let private parseNumber (s : string) =
        Double.Parse (s, CultureInfo.InvariantCulture)

    let private safeSelect (s : string) (n : XmlNode) =
        let raw = n.SelectSingleNode s
        if isNull raw then
            raise <| new ArgumentException (sprintf "Tag ‘%s’ is missing." s)
        raw

    let private safeSelectAttribute (s : string) (n : XmlNode) =
        let raw = n.Attributes.[s]
        if isNull raw then
            raise <| new ArgumentException (sprintf "Attribute ‘%s’ is missing." s)
        raw.Value
    
    let private parseVector (v : XmlNode) : Vector =
        let x = parseNumber <| safeSelectAttribute "x" v
        let y = parseNumber <| safeSelectAttribute "y" v
        let z = parseNumber <| safeSelectAttribute "z" v

        { x = x; y = y; z = z }

    let private parsePlanet (v : XmlNode) : Body =
        let getVector s =
            safeSelect s v
            |> parseVector

        let id = safeSelectAttribute "id" v

        let speed = getVector "Speed"
        let acc = getVector "Acc"
        let pos = getVector "Pos"

        let m = parseNumber (safeSelect "Mass" v).InnerText

        let render = safeSelect "render" v
        let visibleRadius =
            safeSelectAttribute "radius" render
            |> parseNumber
        let visibleColor =
            safeSelectAttribute "color" render
            |> ColorTranslator.FromHtml
        
        { speed = speed;
          acc = acc;
          pos = pos;
          m = m;
          id = id;

          visibleRadius = visibleRadius;
          visibleColor = visibleColor }

    let private parsePlanets (v : XmlDocument) : Body list =
        let planets = v.SelectNodes "//Planets/Planet"
        if isNull planets then
            raise <| new ArgumentException ("Tag ‘Planets’ is missing.")

        Seq.cast<XmlNode> planets
        |> Seq.map parsePlanet
        |> List.ofSeq

    let public parseFile (fileName : string) : Body list =
        let text =
            use sr = new StreamReader (fileName) in
                sr.ReadToEnd ()

        let doc = new XmlDocument ()
        doc.LoadXml text

        parsePlanets doc
