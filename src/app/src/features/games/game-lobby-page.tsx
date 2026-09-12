import {
  ArrowRight,
  Crown,
  Dices,
  Gamepad2,
  HeartHandshake,
  History,
  LoaderCircle,
  Scissors,
  Sparkles,
} from 'lucide-react'
import { useState } from 'react'
import { useNavigate } from 'react-router'
import {
  useGetCurrentCoupleQuery,
  useGetCurrentScheduledTurnGameQuery,
  useGetProfileQuery,
  useGetTurnGameHistoryQuery,
} from '@/app/api'
import { formatDate } from '@/app/format'
import { useTurnGameHub } from '@/app/turn-game-hub'
import type { TurnGameSummary, TurnGameType } from '@/app/types'
import { EmptyState, LoadingState, Notice } from '@/components/page-state'
import { Button } from '@/components/ui/button'
import { ScheduledGameCard } from './scheduled-game-card'

interface GameOption {
  type: TurnGameType
  title: string
  description: string
  icon: React.ReactNode
}

interface HistoryCardProps {
  game: TurnGameSummary
  currentUserId?: string
  onOpen: () => void
}

const gameOptions: GameOption[] = [
  {
    type: 'RockPaperScissors',
    title: 'Piedra, papel o tijera',
    description: 'El clásico, en privado y al mejor de sus rondas.',
    icon: <Scissors />,
  },
  {
    type: 'HigherNumberWins',
    title: 'El número más alto gana',
    description: 'Elijan del 1 al 6. La elección mayor gana la ronda.',
    icon: <Dices />,
  },
]

export function GameLobbyPage() {
  const navigate = useNavigate()
  const { data: couple, isLoading: coupleLoading } = useGetCurrentCoupleQuery()
  const { data: profile } = useGetProfileQuery()
  const { data: scheduledGame, isLoading: scheduledLoading } =
    useGetCurrentScheduledTurnGameQuery(undefined, { skip: !couple?.isActive })
  const { data: history = [], isLoading: historyLoading } =
    useGetTurnGameHistoryQuery(undefined, { skip: !couple?.isActive })
  const { connectionStatus, createGame, selectScheduledGame, error } = useTurnGameHub()
  const [creating, setCreating] = useState<TurnGameType | null>(null)
  const [selecting, setSelecting] = useState<TurnGameType | null>(null)

  if (coupleLoading || scheduledLoading || historyLoading) return <LoadingState />
  if (!couple?.isActive) {
    return (
      <EmptyState
        icon={<Gamepad2 />}
        title="Necesitan estar entrelazados"
        description="Conecta con tu pareja antes de iniciar una partida."
        action={<Button onClick={() => navigate('/conectar')}>Conectar ahora <ArrowRight /></Button>}
      />
    )
  }

  const createCasualGame = async (type: TurnGameType) => {
    setCreating(type)
    try {
      const game = await createGame(type)
      navigate(`/juego/${game.id}`)
    } catch {
      setCreating(null)
    }
  }

  const selectPlanningGame = async (type: TurnGameType) => {
    const game = scheduledGame?.game
    if (!game) return

    setSelecting(type)
    try {
      const selected = await selectScheduledGame(game.id, type)
      navigate(`/juego/${selected.id}`)
    } catch {
      setSelecting(null)
    }
  }

  return (
    <div className="flex flex-col gap-8">
      <div>
        <p className="text-sm text-muted-foreground">Rían, compitan y vuelvan a intentarlo</p>
        <h1 className="mt-1 font-serif text-3xl font-semibold text-ink">Juegos en pareja</h1>
        <p className="mt-2 text-sm leading-6 text-muted-foreground">Aquí conviven el juego especial que decide quién prepara la próxima cita y las partidas casuales que juegan sólo por diversión.</p>
      </div>

      {scheduledGame?.game && (
        <ScheduledGameCard
          game={scheduledGame.game}
          connectionStatus={connectionStatus}
          selecting={selecting}
          onSelect={selectPlanningGame}
          onContinue={() => navigate(`/juego/${scheduledGame.game!.id}`)}
        />
      )}

      {connectionStatus !== 'connected' && <Notice tone="info"><span className="flex items-center gap-2"><LoaderCircle className="size-4 animate-spin" />Conectando los juegos en tiempo real…</span></Notice>}
      {error && <Notice>{error}</Notice>}

      <section>
        <div className="flex items-start gap-3">
          <span className="flex size-10 shrink-0 items-center justify-center rounded-2xl bg-lilac text-plum"><Sparkles /></span>
          <div>
            <h2 className="font-serif text-2xl font-semibold text-ink">Jueguen porque sí</h2>
            <p className="mt-1 text-sm text-muted-foreground">Estas partidas casuales no cambian a quién le corresponde planear la siguiente cita.</p>
          </div>
        </div>
        <div className="mt-4 grid gap-4 sm:grid-cols-2">
          {gameOptions.map((option) => (
            <button
              key={option.type}
              onClick={() => createCasualGame(option.type)}
              disabled={connectionStatus !== 'connected' || Boolean(creating)}
              className="group rounded-3xl border border-lavender bg-white p-5 text-left shadow-card transition hover:-translate-y-0.5 hover:border-plum/30 disabled:opacity-50"
            >
              <span className="flex size-12 items-center justify-center rounded-2xl bg-pink-soft text-plum">{creating === option.type ? <LoaderCircle className="animate-spin" /> : option.icon}</span>
              <h3 className="mt-4 font-serif text-xl font-semibold text-ink">{option.title}</h3>
              <p className="mt-2 text-sm leading-6 text-muted-foreground">{option.description}</p>
              <span className="mt-5 flex items-center gap-2 text-sm font-bold text-plum">Crear partida <ArrowRight className="size-4 transition group-hover:translate-x-1" /></span>
            </button>
          ))}
        </div>
      </section>

      <section>
        <div className="flex items-center gap-3">
          <span className="flex size-10 items-center justify-center rounded-2xl bg-pink-soft text-plum"><History /></span>
          <div>
            <h2 className="font-serif text-2xl font-semibold text-ink">Sus batallas memorables</h2>
            <p className="mt-1 text-sm text-muted-foreground">Victorias, empates y revanchas quedan aquí.</p>
          </div>
        </div>

        {history.length === 0 ? (
          <div className="mt-4 rounded-2xl border border-dashed border-lavender bg-white/60 p-5 text-center text-sm text-muted-foreground">Su primera historia de juego está por comenzar.</div>
        ) : (
          <div className="mt-4 grid gap-3">
            {history.map((game) => (
              <HistoryCard
                key={game.id}
                game={game}
                currentUserId={profile?.id}
                onOpen={() => navigate(`/juego/${game.id}`)}
              />
            ))}
          </div>
        )}
      </section>
    </div>
  )
}

function HistoryCard({ game, currentUserId, onOpen }: HistoryCardProps) {
  const gameName = game.gameType === 'RockPaperScissors'
    ? 'Piedra, papel o tijera'
    : game.gameType === 'HigherNumberWins'
      ? 'El número más alto'
      : 'Juego por elegir'
  const isFinished = game.status === 'Finished'
  const result = !isFinished
    ? 'Partida pendiente'
    : game.winnerId === currentUserId
      ? '¡Te llevaste la corona!'
      : `${game.winnerName ?? 'Tu pareja'} ganó esta vez`

  return (
    <button onClick={onOpen} className="flex w-full items-center gap-4 rounded-2xl border border-lavender bg-white p-4 text-left shadow-sm transition hover:border-plum/30">
      <span className={`flex size-12 shrink-0 items-center justify-center rounded-2xl ${isFinished ? 'bg-sun-soft text-sun-dark' : 'bg-lilac text-plum'}`}>
        {isFinished ? <Crown /> : game.purpose === 'PlanningDecision' ? <HeartHandshake /> : <Gamepad2 />}
      </span>
      <span className="min-w-0 flex-1">
        <span className="flex flex-wrap items-center gap-2">
          <strong className="text-sm text-ink">{gameName}</strong>
          <span className="rounded-full bg-muted px-2 py-1 text-[10px] font-bold uppercase tracking-wide text-muted-foreground">{game.purpose === 'PlanningDecision' ? 'Decidió la cita' : 'Casual'}</span>
        </span>
        <span className="mt-1 block text-xs text-muted-foreground">{result} · {game.roundsPlayed} {game.roundsPlayed === 1 ? 'ronda' : 'rondas'} · {formatDate(game.finishedAtUtc ?? game.createdAtUtc, { dateStyle: 'medium' })}</span>
      </span>
      <ArrowRight className="size-4 shrink-0 text-plum" />
    </button>
  )
}
