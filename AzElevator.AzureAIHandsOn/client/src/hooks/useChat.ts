import { useMutation } from '@tanstack/react-query';
import { askAI, ChatMessage, EndpointType } from '../lib/api';
import { useState } from 'react';

export function useChat(endpoint: EndpointType) {
  const [messages, setMessages] = useState<ChatMessage[]>([]);

  const mutation = useMutation({
    mutationFn: (query: string) => askAI(query, endpoint),
    onSuccess: (response) => {
      setMessages(prev => [
        ...prev,
        { role: 'assistant', content: response.answer }
      ]);
    },
  });

  const sendMessage = (query: string) => {
    setMessages(prev => [...prev, { role: 'user', content: query }]);
    mutation.mutate(query);
  };

  return {
    messages,
    sendMessage,
    isLoading: mutation.isPending,
    error: mutation.error
  };
} 