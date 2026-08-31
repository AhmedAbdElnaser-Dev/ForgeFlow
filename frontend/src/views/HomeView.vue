<script setup>
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
</script>

<template>
  <v-container class="py-10">
    <h1 class="text-h5 font-weight-medium mb-2">Dashboard</h1>
    <p class="text-body-2 text-medium-emphasis mb-8">
      Signed in as {{ auth.user?.email }}. Autodesk and ACC features arrive in a later step.
    </p>

    <v-card class="pa-6">
      <div class="text-caption text-medium-emphasis mb-1">Roles</div>
      <div v-if="auth.user?.roles?.length" class="d-flex ga-2 flex-wrap">
        <v-chip v-for="role in auth.user.roles" :key="role" size="small" color="primary" variant="tonal">
          {{ role }}
        </v-chip>
      </div>
      <div v-else class="text-body-2 text-medium-emphasis">No roles assigned.</div>
    </v-card>
  </v-container>
</template>
