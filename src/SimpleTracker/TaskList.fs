namespace SimpleTracker

type TaskRequirement = {
  Name: string
}

type RequirementDefinition = {
  Name: string
  Count: int
}

type TaskRequirementSection = {
  Definition: RequirementDefinition
  Requirements: TaskRequirement list
}

type TaskItem = {
  Id: int
  IsComplete: bool
  Name: string
  RequirementSections: TaskRequirementSection list
}

type TaskList = {
  FileName: string
  Name: string
  Items: TaskItem list
  Requirements: RequirementDefinition list
}
