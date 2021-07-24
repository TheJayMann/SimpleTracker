namespace SimpleTracker

open System.Threading.Tasks

[<ReferenceEquality>]
type Services = {
  GetTrackerData : unit -> TaskList option Task
}