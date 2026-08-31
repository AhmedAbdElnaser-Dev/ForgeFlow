<script setup>
import { computed } from 'vue'
import { RouterView, useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const route = useRoute()
const router = useRouter()

const showAppBar = computed(() => !route.meta.public)

async function onSignOut() {
  await auth.signOut()
  await router.replace({ name: 'login' })
}
</script>

<template>
  <v-app>
    <v-app-bar v-if="showAppBar" color="surface" flat border="b" height="56">
      <v-app-bar-title class="app-wordmark">Forge<span class="text-primary">Flow</span></v-app-bar-title>

      <template #append>
        <span class="text-body-2 text-medium-emphasis me-3 d-none d-sm-inline">
          {{ auth.user?.email }}
        </span>
        <v-btn variant="text" size="small" prepend-icon="mdi-logout" @click="onSignOut">
          Sign out
        </v-btn>
      </template>
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
