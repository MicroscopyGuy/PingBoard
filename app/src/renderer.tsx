import React from 'react'
import ReactDOM from 'react-dom/client'
import App from "./dashboard/App"
import './index.css'

console.log('👋 This message is being logged by "renderer.ts", included via Vite');

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
)
