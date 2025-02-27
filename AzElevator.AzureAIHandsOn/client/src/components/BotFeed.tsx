import { ChatMessage } from '../lib/api';
import ReactMarkdown from 'react-markdown';

interface BotFeedProps {
  messages: ChatMessage[];
}

export function BotFeed({ messages }: BotFeedProps) {
  return (
    <div className="flex flex-col gap-4 p-4 w-full max-w-4xl mx-auto">
      {messages.map((message, index) => (
        <div
          key={index}
          className={`p-4 rounded-lg ${message.role === 'user'
              ? 'bg-blue-100 ml-auto max-w-[80%]'
              : 'bg-gray-100 mr-auto max-w-[80%] prose prose-sm max-w-none'
            }`}
        >
          {message.role === 'user' ? (
            <div className="text-left">{message.content}</div>
          ) : (
            <div className="overflow-x-auto">
              <div className="text-left break-words whitespace-pre-wrap">
                <ReactMarkdown
                  components={{
                    h1: ({ children }) => <h1 className="text-xl font-bold my-2">{children}</h1>,
                    h2: ({ children }) => <h2 className="text-lg font-semibold my-2">{children}</h2>,
                    blockquote: ({ children }) => (
                      <blockquote className="border-l-4 border-gray-300 pl-4 my-2 italic">
                        {children}
                      </blockquote>
                    ),
                    ul: ({ children }) => <ul className="list-disc ml-4 my-2">{children}</ul>,
                    a: ({ href, children }) => (
                      <a href={href} className="text-blue-600 hover:underline" target="_blank" rel="noopener noreferrer">
                        {children}
                      </a>
                    ),
                    p: ({ children }) => <p className="my-2">{children}</p>,
                    code: ({ children }) => (
                      <code className="bg-gray-200 rounded px-1 py-0.5">{children}</code>
                    ),
                    pre: ({ children }) => (
                      <pre className="bg-gray-800 text-white p-4 rounded-lg overflow-x-auto">
                        {children}
                      </pre>
                    ),
                  }}
                >
                  {message.content}
                </ReactMarkdown>

              </div>
            </div>
          )}
        </div>
      ))}
    </div>
  );
} 