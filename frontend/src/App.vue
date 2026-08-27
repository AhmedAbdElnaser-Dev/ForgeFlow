<script setup>
import { computed } from 'vue'
import { RouterView, useRoute } from 'vue-router'

const route = useRoute()
const showAppBar = computed(() => !route.meta.public)
</script>

<template>
  <v-app>
    <v-app-bar v-if="showAppBar" color="surface" flat border="b" height="56">
      <v-app-bar-title class="app-wordmark">Forge<span class="text-primary">Flow</span></v-app-bar-title>
    </v-app-bar>

    <v-main>
      <RouterView />
    </v-main>
  </v-app>
</template>

<style scoped>
.app-wordmark {
  font-size: 1rem;
  font-weight: 600;
  letter-spacing: -0.01em;
}
</style>
