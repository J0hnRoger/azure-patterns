export interface ChatMessage {
  role: 'user' | 'assistant';
  content: string;
}

interface ApiResponse {
  answer: string;
  tokenCount: number;
}

export type EndpointType = 'marketer' | 'chat';

export async function askAI(query: string, endpoint: EndpointType): Promise<ApiResponse> {
  const url = endpoint === 'marketer' 
    ? 'http://localhost:5236/marketer?llm=local'
    : 'http://localhost:5236/ask';

  const response = await fetch(url, {
    method: 'POST',
    headers: {
      'Accept': 'application/json',
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ query })
  });

  if (!response.ok) {
    throw new Error('Failed to fetch response');
  }

  const data = await response.text();
  return JSON.parse(data);
} 