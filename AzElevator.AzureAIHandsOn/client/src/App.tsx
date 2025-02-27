import './App.css'
import { IAForm } from './features/ia-chat/ia-form'
import { BotFeed } from './components/BotFeed'
import { useChat } from './hooks/useChat'
import { EndpointSelector } from './components/EndpointSelector'
import { useState } from 'react'
import { EndpointType } from './lib/api'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'

// Créer une instance de QueryClient
const queryClient = new QueryClient();

// Séparer le contenu de l'App pour le wrapper avec QueryClientProvider
function AppContent() {
  const [endpoint, setEndpoint] = useState<EndpointType>('marketer');
  const { messages, sendMessage, isLoading } = useChat(endpoint);

  return (
    <>
      <h1 className='text-3xl font-bold mb-4'>John Bot</h1>
      <div className='flex flex-col w-full h-full gap-4'>
        <div className="w-full flex justify-end px-4">
          <EndpointSelector 
            value={endpoint} 
            onChange={setEndpoint} 
          />
        </div>
        <BotFeed messages={messages} />
        <IAForm onSubmit={sendMessage} isLoading={isLoading} />
      </div>
    </>
  )
}

// Composant principal qui wrap le contenu avec QueryClientProvider
function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AppContent />
    </QueryClientProvider>
  )
}

export default App
