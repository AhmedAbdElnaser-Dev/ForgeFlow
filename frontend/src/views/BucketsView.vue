<script setup>
import { onMounted, ref } from 'vue'
import * as bucketService from '@/services/bucketService'

const headers = [
  { title: 'Bucket', key: 'bucketKey' },
  { title: 'Retention', key: 'policyKey', width: 150 },
  { title: 'Created', key: 'createdAtUtc', width: 210 },
  { title: 'Actions', key: 'actions', sortable: false, align: 'end', width: 90 },
]

const retentionOptions = [
  { title: 'Transient — deleted after 24 hours', value: 'Transient' },
  { title: 'Temporary — deleted after 30 days', value: 'Temporary' },
  { title: 'Persistent — kept until deleted', value: 'Persistent' },
]

const retentionColors = {
  transient: 'warning',
  temporary: 'info',
  persistent: 'primary',
}

const buckets = ref([])
const isLoading = ref(false)
const loadError = ref('')
const search = ref('')

const isCreateOpen = ref(false)
const isCreating = ref(false)
const createError = ref('')
const createForm = ref(null)
const newBucket = ref({ name: '', retention: 'Transient' })

const bucketToDelete = ref(null)
const isDeleting = ref(false)

const notice = ref('')
const noticeColor = ref('success')
const isNoticeOpen = ref(false)

const nameRules = [
  (value) => !!value || 'Name is required',
  (value) => value.length >= 3 || 'At least 3 characters',
  (value) => /^[a-z0-9._-]+$/.test(value) || "Lowercase letters, numbers, '.', '-' or '_' only",
]

function showNotice(message, color = 'success') {
  notice.value = message
  noticeColor.value = color
  isNoticeOpen.value = true
}

function messageFrom(error, fallback) {
  return error.response?.data?.detail ?? error.response?.data?.title ?? fallback
}

async function loadBuckets() {
  isLoading.value = true
  loadError.value = ''

  try {
    buckets.value = await bucketService.getBuckets()
  } catch (error) {
    loadError.value = messageFrom(error, 'Could not load buckets.')
  } finally {
    isLoading.value = false
  }
}

function openCreate() {
  newBucket.value = { name: '', retention: 'Transient' }
  createError.value = ''
  isCreateOpen.value = true
}

async function submitCreate() {
  const { valid } = await createForm.value.validate()
  if (!valid) {
    return
  }

  isCreating.value = true
  createError.value = ''

  try {
    const created = await bucketService.createBucket(newBucket.value)
    buckets.value = [...buckets.value, created]
    isCreateOpen.value = false
    showNotice(`Created ${created.bucketKey}`)
  } catch (error) {
    createError.value = messageFrom(error, 'Could not create the bucket.')
  } finally {
    isCreating.value = false
  }
}

async function confirmDelete() {
  isDeleting.value = true

  try {
    const { bucketKey } = bucketToDelete.value
    await bucketService.deleteBucket(bucketKey)
    buckets.value = buckets.value.filter((bucket) => bucket.bucketKey !== bucketKey)
    bucketToDelete.value = null
    showNotice(`Deleted ${bucketKey}`)
  } catch (error) {
    showNotice(messageFrom(error, 'Could not delete the bucket.'), 'error')
  } finally {
    isDeleting.value = false
  }
}

function formatDate(value) {
  return value ? new Date(value).toLocaleString() : '—'
}

onMounted(loadBuckets)
</script>

<template>
  <v-container fluid class="fill-height align-start pa-4">
    <v-card class="d-flex flex-column w-100 h-100">
      <v-toolbar color="surface" density="comfortable">
        <v-toolbar-title class="text-subtitle-1 font-weight-medium">Buckets</v-toolbar-title>

        <v-btn
          color="primary"
          variant="flat"
          prepend-icon="mdi-plus"
          class="me-2"
          @click="openCreate"
        >
          New bucket
        </v-btn>
      </v-toolbar>

      <v-alert
        v-if="loadError"
        type="error"
        variant="tonal"
        rounded="0"
        :text="loadError"
      >
        <template #append>
          <v-btn variant="text" size="small" @click="loadBuckets">Retry</v-btn>
        </template>
      </v-alert>

      <v-text-field
        v-model="search"
        placeholder="Filter buckets"
        prepend-inner-icon="mdi-magnify"
        variant="solo-filled"
        density="compact"
        flat
        hide-details
        clearable
        class="ma-3 flex-0-0"
      />

      <v-data-table
        class="buckets__table"
        :headers="headers"
        :items="buckets"
        :search="search"
        :loading="isLoading"
        item-value="bucketKey"
      >
        <template #[`item.bucketKey`]="{ item }">
          <span class="text-body-2">{{ item.bucketKey }}</span>
        </template>

        <template #[`item.policyKey`]="{ item }">
          <v-chip
            :color="retentionColors[item.policyKey]"
            size="small"
            variant="tonal"
            :text="item.policyKey"
          />
        </template>

        <template #[`item.createdAtUtc`]="{ item }">
          <span class="text-body-2 text-medium-emphasis">{{ formatDate(item.createdAtUtc) }}</span>
        </template>

        <template #[`item.actions`]="{ item }">
          <v-btn
            icon="mdi-delete-outline"
            color="error"
            variant="text"
            size="small"
            :aria-label="`Delete ${item.bucketKey}`"
            @click="bucketToDelete = item"
          />
        </template>

        <template #loading>
          <v-skeleton-loader type="table-row@4" />
        </template>

        <template #no-data>
          <v-empty-state
            icon="mdi-database-off-outline"
            title="No buckets yet"
            text="Create one to start storing models."
          >
            <template #actions>
              <v-btn color="primary" variant="tonal" @click="openCreate">New bucket</v-btn>
            </template>
          </v-empty-state>
        </template>
      </v-data-table>
    </v-card>

    <v-dialog v-model="isCreateOpen" max-width="460">
      <v-card title="New bucket">
        <v-card-text>
          <v-alert
            v-if="createError"
            type="error"
            variant="tonal"
            density="compact"
            class="mb-4"
            :text="createError"
          />

          <v-form ref="createForm" @submit.prevent="submitCreate">
            <v-text-field
              v-model="newBucket.name"
              label="Name"
              variant="outlined"
              :rules="nameRules"
              :disabled="isCreating"
              hint="Your client id is added as a prefix automatically."
              persistent-hint
              autofocus
            />

            <v-select
              v-model="newBucket.retention"
              :items="retentionOptions"
              label="Retention"
              variant="outlined"
              :disabled="isCreating"
              class="mt-6"
              persistent-hint
              hint="Fixed at creation and cannot be changed later."
            />
          </v-form>
        </v-card-text>

        <v-card-actions>
          <v-spacer />
          <v-btn variant="text" :disabled="isCreating" @click="isCreateOpen = false">Cancel</v-btn>
          <v-btn color="primary" variant="flat" :loading="isCreating" @click="submitCreate">
            Create
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog
      :model-value="bucketToDelete !== null"
      max-width="460"
      @update:model-value="bucketToDelete = null"
    >
      <v-card title="Delete bucket" :subtitle="bucketToDelete?.bucketKey">
        <v-card-text>
          <v-alert
            type="warning"
            variant="tonal"
            density="compact"
            text="Every model inside is deleted with it. This cannot be undone."
          />
        </v-card-text>

        <v-card-actions>
          <v-spacer />
          <v-btn variant="text" :disabled="isDeleting" @click="bucketToDelete = null">Cancel</v-btn>
          <v-btn color="error" variant="flat" :loading="isDeleting" @click="confirmDelete">
            Delete
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-snackbar v-model="isNoticeOpen" :color="noticeColor" :timeout="4000" :text="notice" />
  </v-container>
</template>

<style scoped>
/* The table fills what the toolbar and filter leave behind. min-height lets it shrink
   below its content, which is what makes the body scroll instead of the page. */
.buckets__table {
  display: flex;
  flex-direction: column;
  flex: 1 1 auto;
  min-height: 0;
}

.buckets__table :deep(.v-table__wrapper) {
  flex: 1 1 auto;
  overflow-y: auto;
}
</style>
