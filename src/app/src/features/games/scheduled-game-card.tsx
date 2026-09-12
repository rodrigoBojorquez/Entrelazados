import { ArrowRight, Dices, HeartHandshake, LoaderCircle, Scissors } from 'lucide-react'
import type { ScheduledTurnGame, TurnGameType } from '@/app/types'
import { Button } from '@/components/ui/button'

interface ScheduledGameCardProps {
  game: ScheduledTurnGame
  connectionStatus: 'connecting' | 'connected' | 'reconnecting' | 'disconnected'
  selecting: TurnGameType | null
  onSelect: (gameType: TurnGameType) => Promise<void>
  onContinue: () => void
}

export function ScheduledGameCard({
  game,
  connectionStatus,
  selecting,
  onSelect,
  onContinue,
}: ScheduledGameCardProps) {
  const waitingForSelection = game.status === 'AwaitingSelection'

  return (
    <section className="rounded-3xl border border-pink/30 bg-gradient-to-br from-pink-soft to-lilac p-5 shadow-card sm:p-6">
      <div className="flex items-start gap-3">
        <div className="flex size-11 shrink-0 items-center justify-center rounded-2xl bg-white text-plum shadow-sm"><HeartHandshake /></div>
        <div>
          <p className="text-xs font-bold uppercase tracking-wider text-plum">El siguiente capítulo</p>
          <h2 className="mt-1 font-serif text-2xl font-semibold text-ink">
            {waitingForSelection ? 'Elijan cómo decidir quién planea' : 'Su partida ya está lista'}
          </h2>
          <p className="mt-2 text-sm leading-6 text-muted-foreground">
            {waitingForSelection
              ? 'El primero en elegir define el minijuego para ambos. El ganador preparará la próxima cita.'
              : 'Entren los dos: la primera ronda comenzará cuando ambos estén conectados.'}
          </p>
        </div>
      </div>

      {waitingForSelection ? (
        <div className="mt-5 grid gap-3 sm:grid-cols-2">
          <button
            onClick={() => onSelect('RockPaperScissors')}
            disabled={connectionStatus !== 'connected' || selecting !== null}
            className="rounded-2xl border border-white bg-white/80 p-4 text-left transition hover:-translate-y-0.5 hover:border-plum/30 disabled:opacity-50"
          >
            {selecting === 'RockPaperScissors' ? <LoaderCircle className="size-5 animate-spin text-plum" /> : <Scissors className="size-5 text-plum" />}
            <p className="mt-3 font-semibold text-ink">Piedra, papel o tijera</p>
          </button>
          <button
            onClick={() => onSelect('HigherNumberWins')}
            disabled={connectionStatus !== 'connected' || selecting !== null}
            className="rounded-2xl border border-white bg-white/80 p-4 text-left transition hover:-translate-y-0.5 hover:border-plum/30 disabled:opacity-50"
          >
            {selecting === 'HigherNumberWins' ? <LoaderCircle className="size-5 animate-spin text-plum" /> : <Dices className="size-5 text-plum" />}
            <p className="mt-3 font-semibold text-ink">El número más alto</p>
          </button>
        </div>
      ) : (
        <Button onClick={onContinue} className="mt-5 w-full">Entrar a la partida <ArrowRight /></Button>
      )}
    </section>
  )
}
