import { defineConfig } from 'vite';

// https://vitejs.dev/config
export default defineConfig({
    resolve: {
        dedupe: ['react', 'react-dom'],
      },
});
