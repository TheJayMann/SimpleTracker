namespace SimpleTracker

type TaskRequirement = {
  Name: string
}

type TaskRequirementSection = {
  Name: string
  Requirements: TaskRequirement list
}

type TaskItem = {
  Id: int
  IsComplete: bool
  Name: string
  RequirementSections: TaskRequirementSection list
}

type RequirementDefinition = {
  Name: string
  Count: int
}

type TaskList = {
  FileName: string
  Name: string
  Items: TaskItem list
  Requirements: RequirementDefinition list
}