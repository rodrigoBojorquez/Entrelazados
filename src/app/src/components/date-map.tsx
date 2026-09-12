import { useEffect } from 'react'
import type { MapMouseEvent } from 'maplibre-gl'
import type { ItineraryItem } from '@/app/types'
import {
  Map,
  MapControls,
  MapMarker,
  MarkerContent,
  MarkerTooltip,
  useMap,
} from '@/components/ui/map'

interface Coordinates {
  latitude: number
  longitude: number
}

interface DateMapProps {
  items: ItineraryItem[]
  className?: string
}

interface ClickListenerProps {
  onChange: (coordinates: Coordinates) => void
}

interface LocationPickerProps {
  value: Coordinates | null
  onChange: (coordinates: Coordinates) => void
}

const fallbackCenter: [number, number] = [-86.8515, 21.1619]

function averageCenter(points: Coordinates[]): [number, number] {
  if (points.length === 0) return fallbackCenter
  return [
    points.reduce((sum, point) => sum + point.longitude, 0) / points.length,
    points.reduce((sum, point) => sum + point.latitude, 0) / points.length,
  ]
}

export function DateMap({ items, className = 'h-64' }: DateMapProps) {
  const points = items.flatMap((item) => item.latitude === null || item.longitude === null
    ? []
    : [{ latitude: item.latitude, longitude: item.longitude, item }])
  const center = averageCenter(points)

  if (points.length === 0) return null

  return (
    <div className={`overflow-hidden rounded-2xl border border-lavender ${className}`}>
      <Map center={center} zoom={12} theme="light">
        <MapControls showZoom showFullscreen />
        {points.map(({ item, latitude, longitude }) => (
          <MapMarker key={item.id} latitude={latitude} longitude={longitude}>
            <MarkerContent>
              <span className="flex size-9 items-center justify-center rounded-full border-2 border-white bg-plum text-xs font-bold text-white shadow-lg">{item.order}</span>
            </MarkerContent>
            <MarkerTooltip>{item.name}</MarkerTooltip>
          </MapMarker>
        ))}
      </Map>
    </div>
  )
}

function ClickListener({ onChange }: ClickListenerProps) {
  const { map } = useMap()

  useEffect(() => {
    if (!map) return
    const onClick = (event: MapMouseEvent) => onChange({
      latitude: Number(event.lngLat.lat.toFixed(6)),
      longitude: Number(event.lngLat.lng.toFixed(6)),
    })
    map.on('click', onClick)
    return () => { map.off('click', onClick) }
  }, [map, onChange])

  return null
}

export function LocationPicker({ value, onChange }: LocationPickerProps) {
  const center: [number, number] = value ? [value.longitude, value.latitude] : fallbackCenter
  return (
    <div className="h-56 overflow-hidden rounded-2xl border border-lavender">
      <Map center={center} zoom={value ? 14 : 11} theme="light">
        <ClickListener onChange={onChange} />
        <MapControls showZoom showLocate onLocate={({ latitude, longitude }) => onChange({ latitude, longitude })} />
        {value && (
          <MapMarker latitude={value.latitude} longitude={value.longitude} draggable onDragEnd={({ lat, lng }) => onChange({ latitude: lat, longitude: lng })}>
            <MarkerContent><span className="block size-5 rounded-full border-4 border-white bg-pink shadow-lg" /></MarkerContent>
          </MapMarker>
        )}
      </Map>
    </div>
  )
}
