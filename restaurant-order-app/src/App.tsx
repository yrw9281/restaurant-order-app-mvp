import { useMemo, useState } from 'react'
import { useDisclosure } from '@heroui/react'
import type { MenuItem, OrderLine, Order, MeatOption } from './models'
import { DEFAULT_MENU } from './models'
import { useLocalState } from './hooks/useLocalState'
import { generateOrderId, getItemPrice } from './hooks/utils'
import {
  Header,
  BottomNavigation,
  SearchBar,
  MenuList,
  OrderList,
  CartModal,
  OrderDetailModal,
  MeatSelectionModal,
} from './components'

export default function App() {
  // Tabs: menu vs history
  const [tab, setTab] = useState<'menu' | 'history'>('menu')
  const [dark, setDark] = useLocalState<boolean>('pref-dark', false)

  // Menu and filters
  const [menu] = useLocalState<MenuItem[]>('menu', DEFAULT_MENU)
  const [menuQuery, setMenuQuery] = useState('')

  // Current order
  const [cart, setCart] = useLocalState<OrderLine[]>('cart', [])

  // Orders history
  const [orders, setOrders] = useLocalState<Order[]>('orders', [])
  const [histQuery, setHistQuery] = useState('')

  // Modal states
  const { isOpen: isCartOpen, onOpen: onCartOpen, onClose: onCartClose } = useDisclosure()
  const { isOpen: isOrderDetailOpen, onOpen: onOrderDetailOpen, onClose: onOrderDetailClose } = useDisclosure()
  const { isOpen: isMeatSelectionOpen, onOpen: onMeatSelectionOpen, onClose: onMeatSelectionClose } = useDisclosure()
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null)
  const [selectedMenuItem, setSelectedMenuItem] = useState<MenuItem | null>(null)

  const filteredMenu = useMemo(() => {
    const q = menuQuery.trim().toLowerCase()
    if (!q) return menu
    return menu.filter(m => m.name.toLowerCase().includes(q) || m.number.includes(q))
  }, [menu, menuQuery])

  const cartItemsCount = useMemo(() => cart.reduce((s, l) => s + l.quantity, 0), [cart])

  const filteredOrders = useMemo(() => {
    const q = histQuery.trim().toLowerCase()
    if (!q) return orders
    return orders.filter(o =>
      o.items.some(i =>
        i.name.toLowerCase().includes(q) ||
        i.number.includes(q) ||
        (i.notes || '').toLowerCase().includes(q)
      )
    )
  }, [orders, histQuery])

  function addToCart(item: MenuItem, meat?: MeatOption) {
    // If item has available meats and no meat is selected, open meat selection modal
    if (item.availableMeats && item.availableMeats.length > 0 && !meat) {
      setSelectedMenuItem(item)
      onMeatSelectionOpen()
      return
    }

    const itemPrice = getItemPrice(item, meat)
    if (itemPrice === 0) return // Skip items without price

    setCart(prev => {
      const itemKey = meat ? `${item.number}-${meat}` : item.number
      const displayName = meat ? `${item.name} (${meat})` : item.name
      
      const idx = prev.findIndex(l => l.number === itemKey)
      if (idx >= 0) {
        const copy = [...prev]
        copy[idx] = { ...copy[idx], quantity: copy[idx].quantity + 1 }
        return copy
      }
      return [...prev, { 
        number: itemKey, 
        name: displayName, 
        unitPrice: itemPrice, 
        quantity: 1,
        selectedMeat: meat
      }]
    })
  }

  function selectMeatAndAddToCart(meat: MeatOption) {
    if (selectedMenuItem) {
      addToCart(selectedMenuItem, meat)
      setSelectedMenuItem(null)
      onMeatSelectionClose()
    }
  }

  function updateQty(number: string, qty: number) {
    if (qty <= 0) return removeLine(number)
    setCart(prev => prev.map(l => (l.number === number ? { ...l, quantity: qty } : l)))
  }
  
  function updateNotes(number: string, notes: string) {
    setCart(prev => prev.map(l => (l.number === number ? { ...l, notes } : l)))
  }
  
  function removeLine(number: string) {
    setCart(prev => prev.filter(l => l.number !== number))
  }
  
  function clearCart() {
    setCart([])
  }

  function submitOrder() {
    if (!cart.length) return
    const total = cart.reduce((s, l) => s + l.unitPrice * l.quantity, 0)
    const order: Order = {
      id: generateOrderId(),
      timestamp: Date.now(),
      items: cart,
      total,
    }
    setOrders(prev => [order, ...prev])
    clearCart()
    onCartClose()
    setTab('history')
  }

  function viewOrderDetails(order: Order) {
    setSelectedOrder(order)
    onOrderDetailOpen()
  }

  return (
    <div className={`${dark ? 'dark' : ''} min-h-screen bg-background text-foreground`}>
      <Header dark={dark} onDarkChange={setDark} />

      {/* Main Content */}
      <div className="pb-20">
        {tab === 'menu' ? (
          <div className="p-4">
            <SearchBar 
              placeholder="Search menu items..." 
              value={menuQuery} 
              onValueChange={setMenuQuery} 
            />
            <MenuList 
              filteredMenu={filteredMenu} 
              onAddToCart={addToCart} 
            />
          </div>
        ) : (
          <div className="p-4">
            <SearchBar 
              placeholder="Search orders..." 
              value={histQuery} 
              onValueChange={setHistQuery} 
            />
            <OrderList 
              orders={filteredOrders} 
              onOrderClick={viewOrderDetails} 
            />
          </div>
        )}
      </div>

      <BottomNavigation
        tab={tab}
        onTabChange={setTab}
        cartItemsCount={cartItemsCount}
        onCartOpen={onCartOpen}
      />

      <CartModal
        isOpen={isCartOpen}
        onClose={onCartClose}
        cart={cart}
        onUpdateQty={updateQty}
        onUpdateNotes={updateNotes}
        onRemoveLine={removeLine}
        onClearCart={clearCart}
        onSubmitOrder={submitOrder}
      />

      <OrderDetailModal
        isOpen={isOrderDetailOpen}
        onClose={onOrderDetailClose}
        order={selectedOrder}
      />

      <MeatSelectionModal
        isOpen={isMeatSelectionOpen}
        onClose={onMeatSelectionClose}
        item={selectedMenuItem}
        onSelectMeat={selectMeatAndAddToCart}
      />
    </div>
  )
}
