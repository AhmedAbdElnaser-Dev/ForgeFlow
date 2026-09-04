import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import ProfileView from '@/views/ProfileView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'profile',
      component: ProfileView,
      meta: { title: 'Profile', icon: 'mdi-account-outline', nav: true, order: 1 },
    },
    {
      path: '/folders',
      name: 'folders',
      component: () => import('@/views/FoldersView.vue'),
      meta: { title: 'Folders', icon: 'mdi-folder-outline', nav: true, order: 3 },
    },
    {
      path: '/folders/:name',
      name: 'folder',
      component: () => import('@/views/FolderView.vue'),
      meta: { title: 'Folder' },
    },
    {
      path: '/buckets',
      name: 'buckets',
      component: () => import('@/views/BucketsView.vue'),
      meta: {
        title: 'Buckets',
        icon: 'mdi-database-outline',
        nav: true,
        order: 2,
        roles: ['Admin'],
      },
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('@/views/LoginView.vue'),
      meta: { public: true },
    },
  ],
})

router.beforeEach(async (to) => {
  const auth = useAuthStore()

  // The cookie lives in the browser, so on a fresh load only the API knows who we are.
  await auth.restoreSession()

  if (!to.meta.public && !auth.isAuthenticated) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }

  if (to.name === 'login' && auth.isAuthenticated) {
    return { name: 'profile' }
  }

  // Hiding a nav link is not access control; the route has to check as well.
  if (to.meta.roles && !auth.hasAnyRole(to.meta.roles)) {
    return { name: 'profile' }
  }

  return true
})

export default router
