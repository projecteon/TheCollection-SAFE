import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// Fable (the `fable` dotnet tool) compiles F# -> JS into ./output; Vite bundles
// that JS (entry: output/App.js, referenced from index.html). Vite runs with cwd
// = src/Client (launched by `dotnet fable ... --run npx vite` from Build.fs), so
// this config is found automatically and paths are relative to src/Client.
// Backend (Saturn) dev port — hardcoded in src/Server/Config.fs (Port = 8085us).
const SERVER = "http://localhost:8085";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  build: {
    // Production SPA lands in deploy/public, served by Saturn (guarantee: prod output).
    outDir: "../../deploy/public",
    emptyOutDir: true,
  },
  server: {
    port: 8080,
    // Reproduce the old webpack devServer proxy (guarantee: dev proxy + ports).
    proxy: {
      "/api": { target: SERVER, changeOrigin: true },
      "/resource": { target: SERVER, changeOrigin: true },
      "/socket": { target: SERVER, ws: true },
    },
    // Fable owns .fs -> .fs.js compilation; let it, not Vite, watch F# sources.
    watch: { ignored: ["**/*.fs"] },
  },
  // publicDir defaults to src/Client/public → ServiceWorker.js, manifest.webmanifest,
  // Images/, svg/, favicon, offline.html copied to outDir as-is (guarantee: assets).
});
