import { useMemo } from 'react'
import type { MenuItem, MenuCategory } from '../models'
import { MenuItemCard } from './MenuItemCard'

interface MenuListProps {
  filteredMenu: MenuItem[]
  onAddToCart: (item: MenuItem) => void
}

export function MenuList({ filteredMenu, onAddToCart }: MenuListProps) {
  const groupedMenu = useMemo(() => {
    const categories: Record<MenuCategory, MenuItem[]> = {
      'soups': [],
      'salads': [],
      'fried': [],
      'rice-noodles': [],
      'drinks': [],
      'chinese': []
    }
    
    filteredMenu.forEach(item => {
      categories[item.category].push(item)
    })
    
    return categories
  }, [filteredMenu])

  const categoryLabels: Record<MenuCategory, string> = {
    'soups': 'Soups (ဟင်းချို)',
    'salads': 'Salads (သုပ်)',
    'fried': 'Fried (ကြော်)',
    'rice-noodles': 'Rice & Noodles (ထမင်း/ခေါက်ဆွဲ)',
    'drinks': 'Drinks (အရည်)',
    'chinese': 'Chinese (တရုတ်)'
  }

  if (filteredMenu.length === 0) {
    return (
      <div className="text-center py-12">
        <p className="text-default-500">No menu items found</p>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {Object.entries(groupedMenu).map(([category, items]) => {
        if (items.length === 0) return null
        
        return (
          <div key={category}>
            <h2 className="text-lg font-semibold mb-3 text-primary">
              {categoryLabels[category as MenuCategory]}
            </h2>
            <div className="space-y-3">
              {items.map(item => (
                <MenuItemCard 
                  key={item.number}
                  item={item}
                  onAddToCart={onAddToCart}
                />
              ))}
            </div>
          </div>
        )
      })}
    </div>
  )
}
