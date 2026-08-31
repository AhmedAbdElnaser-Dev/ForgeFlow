<script setup>
import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()

const initials = computed(() => auth.user?.email?.slice(0, 2).toUpperCase() ?? '')
</script>

<template>
  <v-container class="profile py-10">
    <h1 class="text-h5 font-weight-medium mb-1">Profile</h1>
    <p class="text-body-2 text-medium-emphasis mb-8">Your ForgeFlow account.</p>

    <v-card class="pa-6">
      <div class="d-flex align-center ga-4 mb-6">
        <v-avatar color="primary" size="52">
          <span class="text-subtitle-1 font-weight-bold">{{ initials }}</span>
        </v-avatar>
        <div class="min-width-0">
          <div class="text-subtitle-1 font-weight-medium text-truncate">
            {{ auth.user?.email }}
          </div>
          <div class="text-caption text-medium-emphasis">Signed in</div>
        </div>
      </div>

      <v-divider class="mb-6" />

      <dl class="profile__details">
        <dt class="text-caption text-medium-emphasis">Email</dt>
        <dd class="text-body-2 text-truncate">{{ auth.user?.email }}</dd>

        <dt class="text-caption text-medium-emphasis">User ID</dt>
        <dd class="text-body-2 text-truncate">{{ auth.user?.id }}</dd>

        <dt class="text-caption text-medium-emphasis">Roles</dt>
        <dd>
          <div v-if="auth.user?.roles?.length" class="d-flex ga-2 flex-wrap">
            <v-chip
              v-for="role in auth.user.roles"
              :key="role"
              size="small"
              color="primary"
              variant="tonal"
            >
              {{ role }}
            </v-chip>
          </div>
          <span v-else class="text-body-2 text-medium-emphasis">No roles assigned</span>
        </dd>
      </dl>
    </v-card>
  </v-container>
</template>

<style scoped>
.profile {
  max-width: 720px;
  margin-inline: 0;
}

.profile__details {
  display: grid;
  grid-template-columns: 120px 1fr;
  gap: 16px 24px;
  align-items: center;
  margin: 0;
}

.profile__details dd {
  margin: 0;
  min-width: 0;
}

@media (max-width: 599px) {
  .profile__details {
    grid-template-columns: 1fr;
    gap: 4px;
  }

  .profile__details dd {
    margin-bottom: 12px;
  }
}
</style>
