namespace SimpleTracker

open System.Threading.Tasks

[<ReferenceEquality>]
type Services = {
  GetTrackerData : unit -> TrackerItem list option Task
}