export type AuthenticationResponse = {
  tokenType: string
  accessToken: string
  accessTokenExpiresAtUtc: string
  refreshToken: string
  refreshTokenExpiresAtUtc: string
}

export type ApiProblem = {
  type?: string
  title?: string
  status?: number
  detail?: string
  errors?: Array<{ code: string; description: string }>
}

export type UserProfile = {
  id: string
  email: string
  firstName: string
  lastName: string
  fullName: string
  partnerCode: string
  datingPreferences: DatingPreferences
}

export type DatingPreferences = {
  favoriteActivities: string[]
  avoidedActivities: string[]
  foodPreferences: string[]
}

export type Couple = {
  id: string
  partnerOneId: string
  partnerTwoId: string
  beginRelationshipAt: string
  isActive: boolean
}

export type Partner = {
  partnerId: string
  partnerName: string
  partnerPreferences: DatingPreferences
}

export type ActiveSession = {
  id: string
  familyId: string
  createdAtUtc: string
  expiresAtUtc: string
}

export type DateStatus = 'Planning' | 'Booked' | 'Canceled' | 'Attended'

export type ItineraryItem = {
  id: string
  order: number
  name: string
  startsAt: string | null
  latitude: number | null
  longitude: number | null
}

export type DatePlan = {
  id: string
  coupleId: string
  partnerWhoPlannedId: string
  isCollaborative: boolean
  createdAtUtc: string
  scheduledFor: string | null
  status: DateStatus
  itinerary: ItineraryItem[]
}

export type DatePlanSummary = {
  id: string
  partnerWhoPlannedId: string
  plannerName: string
  isCollaborative: boolean
  createdAtUtc: string
  scheduledFor: string | null
  status: DateStatus
  title: string
  itineraryItemCount: number
}

export type TurnGameType = 'RockPaperScissors' | 'HigherNumberWins'
export type TurnGameStatus = 'AwaitingSelection' | 'Ready' | 'InProgress' | 'Finished'
export type TurnGamePurpose = 'Casual' | 'PlanningDecision'
export type TurnGameRoundStatus = 'InProgress' | 'Finished'

export type TurnGameMove = {
  id: string
  playerId: string
  sequence: number
  code: string | null
  playedAtUtc: string
}

export type TurnGameRound = {
  id: string
  number: number
  status: TurnGameRoundStatus
  winnerId: string | null
  startedAtUtc: string
  finishedAtUtc: string | null
  moves: TurnGameMove[]
}

export type TurnGame = {
  id: string
  coupleId: string
  playerOneId: string
  playerTwoId: string
  gameType: TurnGameType | null
  purpose: TurnGamePurpose
  status: TurnGameStatus
  winnerId: string | null
  createdAtUtc: string
  startedAtUtc: string | null
  finishedAtUtc: string | null
  serverTimeUtc: string
  rounds: TurnGameRound[]
}

export type TurnGameSummary = {
  id: string
  gameType: TurnGameType | null
  purpose: TurnGamePurpose
  status: TurnGameStatus
  winnerId: string | null
  winnerName: string | null
  createdAtUtc: string
  finishedAtUtc: string | null
  roundsPlayed: number
}

export type ScheduledTurnGame = {
  id: string
  gameType: TurnGameType | null
  status: TurnGameStatus
  createdAtUtc: string
}

export type CurrentScheduledTurnGame = {
  game: ScheduledTurnGame | null
}

export type TurnGamePresence = {
  turnGameId: string
  connectedPlayerIds: string[]
  bothPlayersConnected: boolean
}

export type HubError = {
  code: string
  description: string
  type: string
  validationErrors?: Array<{ code: string; description: string }> | null
}

export type HubResponse<T> = {
  value: T | null
  error: HubError | null
  isSuccess: boolean
}
