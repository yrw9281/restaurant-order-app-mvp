import { Chip, Switch } from '@heroui/react'

interface HeaderProps {
  dark: boolean
  onDarkChange: (dark: boolean) => void
}

export function Header({ dark, onDarkChange }: HeaderProps) {
  return (
    <div className="sticky top-0 z-50 bg-background/80 backdrop-blur-md border-b border-divider">
      <div className="flex items-center justify-between px-4 py-3">
        <div className="flex items-center gap-3">
          <Chip color="primary" variant="flat" size="sm">MVP</Chip>
          <h1 className="text-lg font-semibold">Restaurant</h1>
        </div>
        <Switch isSelected={dark} onValueChange={onDarkChange} size="sm">
          <span className="text-xs">Dark</span>
        </Switch>
      </div>
    </div>
  )
}
