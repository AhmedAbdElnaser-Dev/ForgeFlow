<script setup>
import { computed } from 'vue'
import { RouterView, useRoute, useRouter } from 'vue-router'
import { useDisplay } from 'vuetify'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const route = useRoute()
const router = useRouter()
const { mdAndUp } = useDisplay()

const showNavigation = computed(() => !route.meta.public)

// Collapse to icons on narrow screens, since there is no header to toggle from.
const isRail = computed(() => !mdAndUp.value)

const navigationItems = computed(() =>
  router
    .getRoutes()
    .filter((candidate) => candidate.meta.nav)
    .filter((candidate) => !candidate.meta.roles || auth.hasAnyRole(candidate.meta.roles)),
)

const initials = computed(() => auth.user?.email?.slice(0, 2).toUpperCase() ?? '')

// Walk meta.parent upwards so a page only has to declare who it sits under.
const breadcrumbs = computed(() => {
  const trail = []

  for (let name = route.name; name; ) {
    const match = router.getRoutes().find((candidate) => candidate.name === name)
    if (!match) {
      break
    }

    trail.unshift({
      title: name === route.name && route.params.name ? route.params.name : match.meta.title,
      to: { name, params: route.params },
      disabled: name === route.name,
    })

    name = match.meta.parent
  }

  return trail
})

async function onSignOut() {
  await auth.signOut()
  await router.replace({ name: 'login' })
}
</script>

<template>
  <v-app>
    <v-navigation-drawer
      v-if="showNavigation"
      :rail="isRail"
      permanent
      color="surface"
      border="e"
      width="248"
    >
      <div class="brand">
        <v-icon icon="mdi-hexagon-multiple-outline" color="primary" size="22" />
        <span v-if="!isRail" class="brand__word">Forge<span class="text-primary">Flow</span></span>
      </div>

      <v-divider />

      <v-list nav density="comfortable" class="pa-2">
        <v-list-item
          v-for="item in navigationItems"
          :key="item.name"
          :to="{ name: item.name }"
          :prepend-icon="item.meta.icon"
          :title="item.meta.title"
          color="primary"
          rounded="lg"
        />
      </v-list>

      <template #append>
        <v-divider />
        <div class="pa-2">
          <v-list-item v-if="!isRail" class="px-2 py-3" density="compact">
            <template #prepend>
              <v-avatar color="primary" size="32">
                <span class="text-caption font-weight-bold">{{ initials }}</span>
              </v-avatar>
            </template>
            <v-list-item-title class="text-body-2 text-truncate">
              {{ auth.user?.email }}
            </v-list-item-title>
          </v-list-item>

          <v-list nav density="comfortable" class="pa-0">
            <v-list-item
              prepend-icon="mdi-logout"
              title="Sign out"
              rounded="lg"
              @click="onSignOut"
            />
          </v-list>
        </div>
      </template>
    </v-navigation-drawer>

    <v-main>
      <v-breadcrumbs v-if="breadcrumbs.length" :items="breadcrumbs" density="compact" class="px-6 pt-4 pb-0">
        <template #divider>
          <v-icon icon="mdi-chevron-right" size="16" />
        </template>
      </v-breadcrumbs>

      <RouterView />
    </v-main>
  </v-app>
</template>

<style scoped>
.brand {
  display: flex;
  align-items: center;
  gap: 10px;
  height: 56px;
  padding-inline: 18px;
}

.brand__word {
  font-size: 1rem;
  font-weight: 600;
  letter-spacing: -0.01em;
  white-space: nowrap;
}
</style>
