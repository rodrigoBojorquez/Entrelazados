import { ArrowRight, Check, Clock3, FileText, Gamepad2, Hand, LoaderCircle, Scissors, Trophy, WifiOff } from 'lucide-react'
import { useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router'
import { useGetCurrentPartnerQuery } from '@/app/api'
import { getUserIdFromToken } from '@/app/auth-storage'
import { useAppSelector } from '@/app/hooks'
import { useTurnGameHub } from '@/app/turn-game-hub'
import type { TurnGameRound, TurnGameType } from '@/app/types'
import { EmptyState, LoadingState, Notice } from '@/components/page-state'
import { Button } from '@/components/ui/button'
import { cn } from '@/lib/utils'

interface RoundPanelProps {
  gameType: TurnGameType
  round: TurnGameRound
  serverTimeUtc: string
  currentUserId: string | null
  partnerFirstName: string
  ownMoveCode: string | null
  opponentHasMoved: boolean
  pendingMove: string | null
  onChoose: (code: string) => Promise<void>
  onExpire: (roundId: string) => Promise<void>
}

interface RoundResultProps {
  round: TurnGameRound
  currentUserId: string | null
  gameType: TurnGameType
  partnerFirstName: string
}

const TIMED_ROUND_SECONDS = 10

function firstName(fullName?: string) {
  return fullName?.trim().split(/\s+/)[0] || null
}

function capitalize(value: string) {
  return value.charAt(0).toLocaleUpperCase('es') + value.slice(1)
}

export function TurnGamePage() {
  const navigate = useNavigate()
  const accessToken = useAppSelector((state) => state.auth.session?.accessToken ?? null)
  const currentUserId = getUserIdFromToken(accessToken)
  const { data: partner } = useGetCurrentPartnerQuery(undefined, { skip: !accessToken })
  const { game, error, presence, connectionStatus, startRound, playMove, expireRound } = useTurnGameHub()
  const [pendingMove, setPendingMove] = useState<string | null>(null)
  const [roundPending, setRoundPending] = useState(false)
  const partnerFirstName = firstName(partner?.partnerName) ?? 'tu pareja'
  const capitalizedPartnerName = capitalize(partnerFirstName)

  const score = useMemo(() => {
    if (!game || !currentUserId) return { mine: 0, theirs: 0 }
    return game.rounds.reduce((total, round) => {
      if (round.winnerId === currentUserId) total.mine += 1
      else if (round.winnerId) total.theirs += 1
      return total
    }, { mine: 0, theirs: 0 })
  }, [game, currentUserId])

  if (!game && connectionStatus !== 'disconnected' && !error) return <LoadingState label="Uniéndote a la partida…" />
  if (!game) {
    return (
      <EmptyState
        icon={<WifiOff />}
        title="No pudimos abrir la partida"
        description={error ?? `Revisa tu conexión o pide un enlace nuevo a ${partnerFirstName}.`}
        action={<Button onClick={() => navigate('/juego')}>Volver a Juego</Button>}
      />
    )
  }

  const activeRound = [...game.rounds].reverse().find((round) => round.status === 'InProgress')
  const latestRound = game.rounds.at(-1)
  const ownMove = activeRound?.moves.find((move) => move.playerId === currentUserId)
  const opponentMove = activeRound?.moves.find((move) => move.playerId !== currentUserId)
  const winnerIsMe = game.winnerId === currentUserId
  const bothPlayersConnected = presence?.bothPlayersConnected ?? false
  const isPlanningDecision = game.purpose === 'PlanningDecision'
  const gameType = game.gameType

  const beginRound = async () => {
    setRoundPending(true)
    try { await startRound() } finally { setRoundPending(false) }
  }

  const choose = async (code: string) => {
    setPendingMove(code)
    try { await playMove(code) } finally { setPendingMove(null) }
  }

  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-start justify-between gap-4">
        <div>
          <p className="text-sm text-muted-foreground">{gameType === 'RockPaperScissors' ? 'Piedra, papel o tijera' : gameType === 'HigherNumberWins' ? 'El número más alto gana' : 'Juego especial pendiente'}</p>
          <h1 className="mt-1 font-serif text-3xl font-semibold text-ink">Primero en ganar 2</h1>
          <p className="mt-2 text-xs font-semibold text-plum">{isPlanningDecision ? 'Esta partida decide quién prepara la próxima cita' : 'Partida casual · el resultado no cambia el turno de la cita'}</p>
        </div>
      </div>

      {connectionStatus === 'reconnecting' && <Notice tone="info">Reconectando la partida… Tus movimientos siguen guardados.</Notice>}
      {error && <Notice>{error}</Notice>}
      {game.status !== 'Finished' && !bothPlayersConnected && <Notice tone="info"><span className="flex items-center gap-2"><LoaderCircle className="size-4 animate-spin" />Esperando a que {partnerFirstName} entre a esta misma partida…</span></Notice>}

      <section className="rounded-3xl border border-lavender bg-white p-5 shadow-card">
        <div className="grid grid-cols-[1fr_auto_1fr] items-center gap-3 text-center">
          <div><span className="text-xs font-bold uppercase tracking-wider text-muted-foreground">Tú</span><p className="mt-1 font-serif text-4xl font-semibold text-plum">{score.mine}</p></div>
          <div className="rounded-full bg-lilac px-3 py-1 text-xs font-bold text-plum">Ronda {activeRound?.number ?? latestRound?.number ?? 0}</div>
          <div><span className="text-xs font-bold uppercase tracking-wider text-muted-foreground">{partnerFirstName}</span><p className="mt-1 font-serif text-4xl font-semibold text-pink">{score.theirs}</p></div>
        </div>
      </section>

      {game.status === 'AwaitingSelection' || gameType === null ? (
        <section className="rounded-3xl border border-lavender bg-white p-6 text-center shadow-card">
          <Gamepad2 className="mx-auto size-10 text-plum" />
          <h2 className="mt-4 font-serif text-2xl font-semibold text-ink">Primero elijan el juego</h2>
          <p className="mt-2 text-sm text-muted-foreground">La elección está disponible para ambos en la pantalla de juegos. La primera respuesta será la definitiva.</p>
          <Button onClick={() => navigate('/juego')} className="mt-5 w-full">Elegir juego <ArrowRight /></Button>
        </section>
      ) : game.status === 'Finished' ? (
        <section className="rounded-3xl border border-lavender bg-white p-6 text-center shadow-card">
          <div className="mx-auto flex size-14 items-center justify-center rounded-2xl bg-sun-soft text-sun-dark"><Trophy /></div>
          <h2 className="mt-4 font-serif text-2xl font-semibold text-ink">{isPlanningDecision ? (winnerIsMe ? 'Tú planificas la próxima cita' : `${capitalizedPartnerName} planifica la próxima cita`) : (winnerIsMe ? '¡Ganaste esta revancha!' : `Esta vez ganó ${partnerFirstName}`)}</h2>
          <p className="mt-2 text-sm leading-6 text-muted-foreground">{isPlanningDecision ? 'El nuevo plan ya fue creado y queda reservado para que la persona ganadora prepare la sorpresa.' : 'Fue una partida para divertirse: el turno de la próxima cita permanece intacto.'}</p>
          <Button onClick={() => navigate(isPlanningDecision ? '/citas' : '/juego')} className="mt-5 w-full">{isPlanningDecision ? 'Ir a Citas' : 'Volver a Juegos'} <ArrowRight /></Button>
        </section>
      ) : activeRound ? (
        <RoundPanel
          key={activeRound.id}
          gameType={gameType}
          round={activeRound}
          serverTimeUtc={game.serverTimeUtc}
          currentUserId={currentUserId}
          partnerFirstName={partnerFirstName}
          ownMoveCode={ownMove?.code ?? null}
          opponentHasMoved={Boolean(opponentMove)}
          pendingMove={pendingMove}
          onChoose={choose}
          onExpire={expireRound}
        />
      ) : (
        <section className="rounded-3xl border border-lavender bg-white p-6 text-center shadow-card">
          {latestRound && <RoundResult round={latestRound} currentUserId={currentUserId} gameType={gameType} partnerFirstName={partnerFirstName} />}
          {!latestRound && <><Gamepad2 className="mx-auto size-10 text-plum" /><h2 className="mt-4 font-serif text-2xl font-semibold text-ink">{bothPlayersConnected ? 'Los dos están aquí' : `Esperando a ${partnerFirstName}`}</h2><p className="mt-2 text-sm text-muted-foreground">{bothPlayersConnected ? 'La primera ronda está por comenzar automáticamente.' : 'La partida iniciará cuando ambos estén conectados.'}</p></>}
          <Button onClick={beginRound} disabled={roundPending || connectionStatus !== 'connected' || !bothPlayersConnected} className="mt-5 w-full">{roundPending ? 'Iniciando…' : latestRound ? 'Siguiente ronda' : bothPlayersConnected ? 'Comenzar ahora' : `Falta ${partnerFirstName}`} <ArrowRight /></Button>
        </section>
      )}
    </div>
  )
}

function RoundPanel({
  gameType,
  round,
  serverTimeUtc,
  currentUserId,
  partnerFirstName,
  ownMoveCode,
  opponentHasMoved,
  pendingMove,
  onChoose,
  onExpire,
}: RoundPanelProps) {
  const choices = gameType === 'RockPaperScissors'
    ? [
        { code: 'rock', label: 'Piedra', icon: <Hand /> },
        { code: 'paper', label: 'Papel', icon: <FileText /> },
        { code: 'scissors', label: 'Tijera', icon: <Scissors /> },
      ]
    : [1, 2, 3, 4, 5, 6].map((number) => ({ code: String(number), label: String(number), icon: null }))
  const hasSubmitted = Boolean(round.moves.find((move) => move.playerId === currentUserId))
  const hasTimedMoves = gameType === 'RockPaperScissors' || gameType === 'HigherNumberWins'
  const secondsLeft = useRoundCountdown(
    round.startedAtUtc,
    serverTimeUtc,
    hasTimedMoves ? TIMED_ROUND_SECONDS : null,
  )
  const [expirationAttempt, setExpirationAttempt] = useState(0)
  const timeIsUp = secondsLeft === 0

  useEffect(() => {
    if (!timeIsUp) return
    let retry: number | undefined
    void onExpire(round.id).catch(() => {
      retry = window.setTimeout(
        () => setExpirationAttempt((attempt) => attempt + 1),
        1000,
      )
    })
    return () => window.clearTimeout(retry)
  }, [expirationAttempt, onExpire, round.id, timeIsUp])

  return (
    <section className="rounded-3xl border border-lavender bg-white p-5 shadow-card sm:p-6">
      <div className="text-center">
        <div className="flex items-center justify-center gap-2">
          <span className="rounded-full bg-pink-soft px-3 py-1 text-xs font-bold text-plum">Ronda {round.number}</span>
          {secondsLeft !== null && (
            <span
              role="timer"
              aria-live="polite"
              aria-label={`${secondsLeft} ${secondsLeft === 1 ? 'segundo restante' : 'segundos restantes'}`}
              className={cn('inline-flex min-w-20 items-center justify-center gap-1.5 rounded-full px-3 py-1 text-xs font-bold tabular-nums', timeIsUp ? 'bg-danger/10 text-danger' : secondsLeft <= 2 ? 'bg-sun-soft text-sun-dark' : 'bg-lilac text-plum')}
            >
              <Clock3 className="size-3.5" aria-hidden="true" />
              0:{String(secondsLeft).padStart(2, '0')}
            </span>
          )}
        </div>
        <h2 className="mt-4 font-serif text-2xl font-semibold text-ink">{hasSubmitted ? 'Elección enviada' : 'Haz tu elección'}</h2>
        <p className="mt-2 text-sm text-muted-foreground">{timeIsUp ? 'Tiempo agotado. Cerrando la ronda…' : opponentHasMoved ? `${capitalize(partnerFirstName)} ya eligió. Su jugada sigue oculta.` : `La elección de ${partnerFirstName} se revelará al terminar la ronda.`}</p>
      </div>
      <div className={cn('mt-6 grid gap-3', gameType === 'RockPaperScissors' ? 'grid-cols-3' : 'grid-cols-3 sm:grid-cols-6')}>
        {choices.map((choice) => (
          <button
            key={choice.code}
            onClick={() => onChoose(choice.code)}
            disabled={hasSubmitted || Boolean(pendingMove) || timeIsUp}
            className={cn('flex min-h-24 flex-col items-center justify-center gap-2 rounded-2xl border text-sm font-bold transition', ownMoveCode === choice.code ? 'border-plum bg-lilac text-plum' : 'border-lavender bg-background text-ink hover:border-plum/50', gameType === 'HigherNumberWins' && 'text-2xl')}
          >
            {pendingMove === choice.code ? <LoaderCircle className="animate-spin" /> : choice.icon}
            {choice.label}
          </button>
        ))}
      </div>
      {hasSubmitted && !timeIsUp && <div className="mt-5 flex items-center justify-center gap-2 rounded-xl bg-success-soft px-3 py-3 text-sm font-semibold text-success"><Check className="size-4" />Esperando a {partnerFirstName}…</div>}
    </section>
  )
}

function RoundResult({ round, currentUserId, gameType, partnerFirstName }: RoundResultProps) {
  const mine = round.moves.find((move) => move.playerId === currentUserId)?.code
  const theirs = round.moves.find((move) => move.playerId !== currentUserId)?.code
  const label = (code?: string | null) => gameType === 'RockPaperScissors'
    ? ({ rock: 'Piedra', paper: 'Papel', scissors: 'Tijera' }[code ?? ''] ?? '—')
    : code ?? '—'
  const result = round.winnerId === null
    ? 'Empate: esta ronda no suma.'
    : round.winnerId === currentUserId
      ? 'Ganaste esta ronda.'
      : `${capitalize(partnerFirstName)} ganó esta ronda.`

  return (
    <div>
      <Trophy className="mx-auto size-9 text-sun-dark" />
      <h2 className="mt-3 font-serif text-2xl font-semibold text-ink">{result}</h2>
      <div className="mt-4 grid grid-cols-2 gap-3">
        <div className="rounded-xl bg-lilac p-3"><p className="text-xs text-muted-foreground">Tú</p><p className="mt-1 font-bold text-plum">{label(mine)}</p></div>
        <div className="rounded-xl bg-pink-soft p-3"><p className="text-xs text-muted-foreground">{capitalize(partnerFirstName)}</p><p className="mt-1 font-bold text-plum">{label(theirs)}</p></div>
      </div>
    </div>
  )
}

function useRoundCountdown(
  startedAtUtc: string,
  serverTimeUtc: string,
  durationSeconds: number | null,
) {
  const [serverClockOffset] = useState(
    () => new Date(serverTimeUtc).getTime() - Date.now(),
  )
  const deadline = useMemo(
    () => durationSeconds === null ? null : new Date(startedAtUtc).getTime() + durationSeconds * 1000,
    [durationSeconds, startedAtUtc],
  )
  const [secondsLeft, setSecondsLeft] = useState(() =>
    deadline === null ? null : Math.max(0, Math.ceil((deadline - (Date.now() + serverClockOffset)) / 1000)),
  )

  useEffect(() => {
    const update = () => setSecondsLeft(
      deadline === null ? null : Math.max(0, Math.ceil((deadline - (Date.now() + serverClockOffset)) / 1000)),
    )
    update()
    const interval = window.setInterval(update, 100)
    return () => window.clearInterval(interval)
  }, [deadline, serverClockOffset])

  return secondsLeft
}
