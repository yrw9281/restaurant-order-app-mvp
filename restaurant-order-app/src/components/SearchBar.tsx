import { Input } from '@heroui/react'

interface SearchBarProps {
  placeholder: string
  value: string
  onValueChange: (value: string) => void
}

export function SearchBar({ placeholder, value, onValueChange }: SearchBarProps) {
  return (
    <div className="mb-4">
      <Input
        placeholder={placeholder}
        value={value}
        onValueChange={onValueChange}
        size="lg"
        className="w-full"
        startContent={
          <svg className="w-5 h-5 text-default-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
        }
      />
    </div>
  )
}
