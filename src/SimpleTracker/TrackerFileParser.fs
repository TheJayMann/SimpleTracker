module SimpleTracker.TrackerFileParser

open FSharp.Data
open System
open FSharpPlus.Operators
open FSharpPlus

let load filename : TaskList option =
  let parseHeaders (headers: _ array) =

    // This takes the remaining header fields after the main name field and attempts to parse
    // out names of requirements sections, as well as count the number of trailing blank sections
    // to determine how many columns each section contains.
    let parseDef header (currentCount, requirements) =
      if header |> String.IsNullOrEmpty 
      then currentCount + 1, requirements
      else 0, { RequirementDefinition.Name = header; Count = currentCount + 1} :: requirements

    // There must be at least two fields in the header, as the first column determines if
    // the task has been completed, and the second column is the name of the task.  Columns
    // beyond the second are parsed as requirements for the task.
    if headers.Length < 2 then None else
      Array.foldBack parseDef headers.[2..] (0, [])
      |> snd
      |> tuple2 headers.[1]
      |> Some

  let parseRow reqDefs (fields: _ array) index =
    let parseReqs (fields: _ array, reqs) { RequirementDefinition.Name = name; Count = count} =
      let mkTaskReq name = { TaskRequirement.Name = name }
      fields.[count..], { TaskRequirementSection.Name = name; Requirements = fields.[..count - 1] |> Seq.filter (not << String.IsNullOrWhiteSpace) |> Seq.map mkTaskReq |> List.ofSeq } :: reqs


    if fields.Length < 2 then None else Some {
      TaskItem.Name = fields.[1]
      Id = index
      IsComplete = fields.[0] |> String.IsNullOrWhiteSpace |> not
      RequirementSections =
        List.fold parseReqs (fields.[2..], []) reqDefs
        |> snd
        |> List.rev
    } 

  let csv = CsvFile.Load(uri = filename)

  let parseRows reqDefs = 
    let getCols (row: CsvRow) = row.Columns
    let parseRow = getCols >> parseRow reqDefs |> flip
    csv.Rows
    |> Seq.mapi parseRow 
    |> Seq.choose id
    |> List.ofSeq
  
  let expandSnd f (a, b) = (a, b, f b)
  let optionalExpandSnd f = expandSnd f |> Option.map

  let mkTaskList (name, reqDefs, items) = {
    TaskList.Name = name
    Items = items
    Requirements = reqDefs
    FileName = filename
  }
  csv.Headers
  >>= parseHeaders
  |> optionalExpandSnd parseRows
  |>> mkTaskList
  
let save taskList = ()