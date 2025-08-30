import type { MeatOption } from './MenuItem'

export type OrderLine = {
  number: string
  name: string
  unitPrice: number
  quantity: number
  notes?: string
  selectedMeat?: MeatOption
}

export type Order = {
  id: string
  timestamp: number
  items: OrderLine[]
  total: number
}
