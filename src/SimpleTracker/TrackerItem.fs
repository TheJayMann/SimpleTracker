namespace SimpleTracker

type TrackerItemRequirement = {
  Description: string
}

type TrackerItemRequirementSection = {
  Description: string
  Requirements: TrackerItemRequirement list
}

type TrackerItem = {
  IsComplete: bool
  Description: string
  RequirementSections: TrackerItemRequirementSection list
}