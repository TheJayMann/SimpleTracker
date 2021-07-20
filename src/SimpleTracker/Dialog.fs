module SimpleTracker.Dialog

open Avalonia.Controls
open FSharp.Control.Tasks.V2

let openFileDialog window = task {
  let filters =  ResizeArray [|
    FileDialogFilter(Name="CSV files", Extensions=ResizeArray [| "csv" |])
    FileDialogFilter(Name="All files", Extensions=ResizeArray [| "*" |])
  |]
  let! results = OpenFileDialog(Title="Load tracker file", Filters=filters).ShowAsync window
  return Seq.tryHead results
}