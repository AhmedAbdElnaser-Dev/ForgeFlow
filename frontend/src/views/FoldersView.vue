<script setup>
import { computed, onMounted, ref } from 'vue'
import * as bucketService from '@/services/bucketService'

const buckets = ref([])
const isLoading = ref(false)
const loadError = ref('')
const search = ref('')

const folders = computed(() => {
  const term = search.value?.trim().toLowerCase() ?? ''

  return buckets.value.filter((bucket) => bucket.bucketKey.toLowerCase().includes(term))
})

function messageFrom(error, fallback) {
  const problem = error.response?.data
  const message = problem?.detail ?? problem?.title

  if (message) {
    return message
  }

  return error.response ? `${fallback} (HTTP ${error.response.status})` : `${fallback} (no response)`
}

async function loadFolders() {
  isLoading.value = true
  loadError.value = ''

  try {
    buckets.value = await bucketService.getFolders()
  } catch (error) {
    loadError.value = messageFrom(error, 'Could not load folders.')
  } finally {
    isLoading.value = false
  }
}

onMounted(loadFolders)
</script>

<template>
  <v-container class="py-8">
    <v-text-field
      v-model="search"
      placeholder="Filter folders"
      prepend-inner-icon="mdi-magnify"
      variant="solo-filled"
      density="compact"
      flat
      hide-details
      clearable
      class="mb-6"
      style="max-width: 360px"
    />

    <v-alert
      v-if="loadError"
      type="error"
      variant="tonal"
      :text="loadError"
      class="mb-6"
    >
      <template #append>
        <v-btn variant="text" size="small" @click="loadFolders">Retry</v-btn>
      </template>
    </v-alert>

    <v-row v-if="isLoading">
      <v-col v-for="placeholder in 4" :key="placeholder" cols="12" sm="6" md="4" lg="3">
        <v-skeleton-loader type="image, list-item-two-line" />
      </v-col>
    </v-row>

    <v-row v-else-if="folders.length">
      <v-col v-for="folder in folders" :key="folder.bucketKey" cols="12" sm="6" md="4" lg="3">
        <v-card
          :to="{ name: 'folder', params: { bucketKey: folder.bucketKey } }"
          class="pa-5 h-100"
          hover
        >
          <v-icon color="primary" icon="mdi-folder-outline" size="40" class="mb-4" />

          <div class="text-body-2 text-truncate mb-1">{{ folder.bucketKey }}</div>

          <v-chip size="x-small" variant="tonal" class="mt-3" :text="folder.policyKey" />
        </v-card>
      </v-col>
    </v-row>

    <v-empty-state
      v-else-if="!loadError"
      icon="mdi-folder-off-outline"
      title="No folders"
      text="An admin must activate a bucket before it appears here."
    />
  </v-container>
</template>
