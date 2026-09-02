<script setup>
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()

const initials = computed(() => auth.user?.email?.slice(0, 2).toUpperCase() ?? '')
</script>

<template>
  <v-container class="py-8">
    <v-card max-width="640">
      <v-list-item class="pa-5">
        <template #prepend>
          <v-avatar color="primary" size="56">
            <span class="text-h6 font-weight-bold">{{ initials }}</span>
          </v-avatar>
        </template>

        <v-list-item-title class="text-h6">{{ auth.user?.email }}</v-list-item-title>
        <v-list-item-subtitle>Signed in</v-list-item-subtitle>
      </v-list-item>

      <v-divider />

      <v-list lines="two" density="comfortable">
        <v-list-item title="Email" :subtitle="auth.user?.email" />
        <v-list-item title="User ID" :subtitle="auth.user?.id" />

        <v-list-item title="Roles">
          <template #subtitle>
            <div v-if="auth.user?.roles?.length" class="d-flex ga-2 flex-wrap mt-1">
              <v-chip
                v-for="role in auth.user.roles"
                :key="role"
                color="primary"
                size="small"
                variant="tonal"
                :text="role"
              />
            </div>
            <span v-else>No roles assigned</span>
          </template>
        </v-list-item>
      </v-list>
    </v-card>
  </v-container>
</template>
