import { useState } from 'react'

export function useLocalState<T>(key: string, initial: T) {
  const [state, setState] = useState<T>(() => {
    try {
      const raw = localStorage.getItem(key)
      return raw ? (JSON.parse(raw) as T) : initial
    } catch {
      return initial
    }
  })
  
  const set = (v: T | ((p: T) => T)) => {
    setState(prev => {
      const next = typeof v === 'function' ? (v as (p: T) => T)(prev) : v
      try { 
        localStorage.setItem(key, JSON.stringify(next)) 
      } catch {}
      return next
    })
  }
  
  return [state, set] as const
}
