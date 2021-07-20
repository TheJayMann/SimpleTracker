module SimpleTracker.Dialog

open Avalonia.Controls
open FSharp.Control.Tasks.V2

let openFileDialog window = task {
  let! results = OpenFileDialog(Title="Load tracker file").ShowAsync window
  return Seq.tryHead results
}