import * as signalR from '@microsoft/signalr'
import { useCallback, useEffect, useRef, useState } from 'react'
import type { HubResponse, TurnGame, TurnGamePresence, TurnGameType } from './types'
import { useAppDispatch, useAppSelector } from './hooks'
import { datePlannerApi } from './api'

type ConnectionStatus = 'connecting' | 'connected' | 'reconnecting' | 'disconnected'

export function useTurnGameHub(turnGameId?: string) {
  const accessToken = useAppSelector((state) => state.auth.session?.accessToken)
  const dispatch = useAppDispatch()
  const connectionRef = useRef<signalR.HubConnection | null>(null)
  const [connectionStatus, setConnectionStatus] = useState<ConnectionStatus>('connecting')
  const [game, setGame] = useState<TurnGame | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [presence, setPresence] = useState<TurnGamePresence | null>(null)
  const [scheduledGameId, setScheduledGameId] = useState<string | null>(null)
  const [selectedGameId, setSelectedGameId] = useState<string | null>(null)

  const unwrap = useCallback(<T,>(response: HubResponse<T>) => {
    if (response.error || !response.value) {
      throw new Error(response.error?.description ?? 'No pudimos completar la acción del juego.')
    }
    return response.value
  }, [])

  useEffect(() => {
    if (!accessToken) return
    let cancelled = false
    const connection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/turn-games', { accessTokenFactory: () => accessToken })
      .withAutomaticReconnect([0, 1500, 3000, 5000])
      .configureLogging(signalR.LogLevel.Warning)
      .build()

    connectionRef.current = connection
    connection.on('TurnGameUpdated', (updated: TurnGame) => {
      setGame(updated)
      if (updated.status === 'Finished') {
        dispatch(datePlannerApi.util.invalidateTags(['TurnGame', 'DatePlan']))
      }
    })
    connection.on('TurnGameFinished', (finished: TurnGame) => {
      setGame(finished)
      dispatch(datePlannerApi.util.invalidateTags(['TurnGame', 'DatePlan']))
    })
    connection.on('TurnGameScheduled', (notification: { turnGameId: string }) => {
      setScheduledGameId(notification.turnGameId)
      dispatch(datePlannerApi.util.invalidateTags(['TurnGame']))
      // Domain notifications can reach SignalR a few milliseconds before the
      // transaction commits. A second invalidation guarantees both screens read
      // the persisted scheduled game after commit.
      window.setTimeout(() => {
        dispatch(datePlannerApi.util.invalidateTags(['TurnGame']))
      }, 500)
    })
    connection.on('TurnGameSelected', (selected: TurnGame) => {
      setGame(selected)
      setSelectedGameId(selected.id)
      dispatch(datePlannerApi.util.invalidateTags(['TurnGame']))
    })
    connection.on('TurnGamePresenceChanged', (updatedPresence: TurnGamePresence) => {
      setPresence(updatedPresence)
    })
    connection.onreconnecting(() => setConnectionStatus('reconnecting'))
    connection.onreconnected(async () => {
      setConnectionStatus('connected')
      if (turnGameId) {
        const response = await connection.invoke<HubResponse<TurnGame>>('JoinTurnGame', turnGameId)
        setGame(unwrap(response))
      }
    })
    connection.onclose(() => setConnectionStatus('disconnected'))

    const start = async () => {
      try {
        setConnectionStatus('connecting')
        await connection.start()
        if (cancelled) return
        setConnectionStatus('connected')
        if (turnGameId) {
          const response = await connection.invoke<HubResponse<TurnGame>>('JoinTurnGame', turnGameId)
          setGame(unwrap(response))
        }
      } catch (reason) {
        if (!cancelled) {
          setConnectionStatus('disconnected')
          setError(reason instanceof Error ? reason.message : 'No pudimos conectar la partida.')
        }
      }
    }

    start()
    return () => {
      cancelled = true
      connectionRef.current = null
      connection.stop()
    }
  }, [accessToken, dispatch, turnGameId, unwrap])

  const invoke = useCallback(async <T,>(method: string, ...args: unknown[]) => {
    const connection = connectionRef.current
    if (!connection || connection.state !== signalR.HubConnectionState.Connected) {
      throw new Error('La conexión del juego todavía no está lista.')
    }
    setError(null)
    try {
      const response = await connection.invoke<HubResponse<T>>(method, ...args)
      return unwrap(response)
    } catch (reason) {
      const message = reason instanceof Error ? reason.message : 'No pudimos completar la jugada.'
      setError(message)
      throw reason
    }
  }, [unwrap])

  const createGame = useCallback(async (gameType: TurnGameType) => {
    const created = await invoke<TurnGame>('CreateTurnGame', gameType)
    setGame(created)
    return created
  }, [invoke])

  const selectScheduledGame = useCallback(async (
    scheduledTurnGameId: string,
    gameType: TurnGameType,
  ) => {
    const selected = await invoke<TurnGame>(
      'SelectScheduledGame',
      scheduledTurnGameId,
      gameType,
    )
    setGame(selected)
    return selected
  }, [invoke])

  const startRound = useCallback(async () => {
    if (!game) return
    setGame(await invoke<TurnGame>('StartRound', game.id))
  }, [game, invoke])

  const playMove = useCallback(async (moveCode: string) => {
    if (!game) return
    setGame(await invoke<TurnGame>('PlayMove', game.id, moveCode))
  }, [game, invoke])

  const expireRound = useCallback(async (roundId: string) => {
    if (!game) return
    setGame(await invoke<TurnGame>('ExpireRound', game.id, roundId))
  }, [game, invoke])

  return {
    game,
    error,
    presence,
    scheduledGameId,
    selectedGameId,
    connectionStatus,
    createGame,
    selectScheduledGame,
    startRound,
    playMove,
    expireRound,
  }
}
