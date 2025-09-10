<script setup lang="ts">
import { HubConnectionBuilder } from "@microsoft/signalr";
import { ref, onMounted } from "vue";

const messages = ref<string[]>([]);
const status = ref("Conectando...");

onMounted(async () => {
  const connection = new HubConnectionBuilder()
    .withUrl("https://localhost:7232//hubs/test") // URL do seu backend .NET
    .withAutomaticReconnect()
    .build();

  connection.on("ReceiveMessage", (user: string, message: string) => {
    messages.value.push(`${user}: ${message}`);
  });

  try {
    await connection.start();
    status.value = "✅ Conectado ao SignalR";
  } catch (err) {
    status.value = "❌ Falha na conexão: " + err;
  }
});
</script>

<template>
  <div style="padding: 2rem; font-family: sans-serif">
    <h1>Cliente Vue + SignalR</h1>
    <p>{{ status }}</p>

    <h2>Mensagens recebidas:</h2>
    <ul>
      <li v-for="(msg, i) in messages" :key="i">{{ msg }}</li>
    </ul>
  </div>
</template>
