import { Badge, Button } from '@heroui/react'

interface BottomNavigationProps {
  tab: 'menu' | 'history'
  onTabChange: (tab: 'menu' | 'history') => void
  cartItemsCount: number
  onCartOpen: () => void
}

export function BottomNavigation({ tab, onTabChange, cartItemsCount, onCartOpen }: BottomNavigationProps) {
  return (
    <div className="fixed bottom-0 left-0 right-0 bg-background/80 backdrop-blur-md border-t border-divider">
      <div className="flex items-center">
        {/* Menu Tab */}
        <button
          className={`flex-1 flex flex-col items-center py-3 px-4 ${
            tab === 'menu' ? 'text-primary' : 'text-default-500'
          }`}
          onClick={() => onTabChange('menu')}
        >
          <svg className="w-6 h-6 mb-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
          </svg>
          <span className="text-xs font-medium">Menu</span>
        </button>

        {/* Cart Button */}
        <div className="flex-1 flex justify-center">
          <Badge
            content={cartItemsCount}
            color="danger"
            isInvisible={cartItemsCount === 0}
            placement="top-right"
          >
            <Button
              color="primary"
              size="lg"
              className="h-14 w-14 rounded-full"
              onPress={onCartOpen}
              isDisabled={cartItemsCount === 0}
            >
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 3h2l.4 2M7 13h10l4-8H5.4m0 0L7 13m0 0l-2.5 5H19" />
              </svg>
            </Button>
          </Badge>
        </div>

        {/* History Tab */}
        <button
          className={`flex-1 flex flex-col items-center py-3 px-4 ${
            tab === 'history' ? 'text-primary' : 'text-default-500'
          }`}
          onClick={() => onTabChange('history')}
        >
          <svg className="w-6 h-6 mb-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <span className="text-xs font-medium">History</span>
        </button>
      </div>
    </div>
  )
}
