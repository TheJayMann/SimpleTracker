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

type RequirementDefinition = {
  Description: string
  Count: int
}

type TrackerList = {
  FileName: string
  Name: string
  Items: TrackerItem list
  Requirements: RequirementDefinition list
}