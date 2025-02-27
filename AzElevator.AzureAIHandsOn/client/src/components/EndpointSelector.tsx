import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { EndpointType } from "@/lib/api"

interface EndpointSelectorProps {
  value: EndpointType;
  onChange: (value: EndpointType) => void;
}

export function EndpointSelector({ value, onChange }: EndpointSelectorProps) {
  return (
    <Select value={value} onValueChange={onChange as (value: string) => void}>
      <SelectTrigger className="w-[180px]">
        <SelectValue className="text-white" placeholder="Select endpoint" />
      </SelectTrigger>
      <SelectContent>
        <SelectItem className="hover:bg-gray-100" value="marketer">Marketer AI</SelectItem>
        <SelectItem value="chat">General Chat</SelectItem>
      </SelectContent>
    </Select>
  )
} 