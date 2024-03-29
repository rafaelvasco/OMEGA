﻿using System.Runtime.InteropServices;

namespace STB
{
	internal unsafe partial class StbTrueType
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct stbtt__hheap_chunk
		{
			public stbtt__hheap_chunk* next;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct stbtt__hheap
		{
			public stbtt__hheap_chunk* head;
			public void* first_free;
			public int num_remaining_in_head_chunk;
		}

		public static void* stbtt__hheap_alloc(stbtt__hheap* hh, ulong size, void* userdata)
		{
			if ((hh->first_free) != null)
			{
				void* p = hh->first_free;
				hh->first_free = *(void**)(p);
				return p;
			}
			else
			{
				if ((hh->num_remaining_in_head_chunk) == (0))
				{
					int count = (int)((size) < (32) ? 2000 : (size) < (128) ? 800 : 100);
					stbtt__hheap_chunk* c = (stbtt__hheap_chunk*)(CRuntime.Malloc((ulong)((ulong)sizeof(stbtt__hheap_chunk) + size * (ulong)(count))));
					if ((c) == (null))
						return (null);
					c->next = hh->head;
					hh->head = c;
					hh->num_remaining_in_head_chunk = (int)(count);
				}
				--hh->num_remaining_in_head_chunk;
				return (sbyte*)(hh->head) + sizeof(stbtt__hheap_chunk) + size * (ulong)hh->num_remaining_in_head_chunk;
			}

		}

		public static void stbtt__hheap_free(stbtt__hheap* hh, void* p)
		{
			*(void**)(p) = hh->first_free;
			hh->first_free = p;
		}

		public static void stbtt__hheap_cleanup(stbtt__hheap* hh, void* userdata)
		{
			stbtt__hheap_chunk* c = hh->head;
			while ((c) != null)
			{
				stbtt__hheap_chunk* n = c->next;
				CRuntime.Free(c);
				c = n;
			}
		}

		public static stbtt__active_edge* stbtt__new_active(stbtt__hheap* hh, stbtt__edge* e, int off_x, float start_point, void* userdata)
		{
			stbtt__active_edge* z = (stbtt__active_edge*)(stbtt__hheap_alloc(hh, (ulong)(sizeof(stbtt__active_edge)), userdata));
			float dxdy = (float)((e->x1 - e->x0) / (e->y1 - e->y0));
			if (z == null)
				return z;
			z->fdx = (float)(dxdy);
			z->fdy = (float)(dxdy != 0.0f ? (1.0f / dxdy) : 0.0f);
			z->fx = (float)(e->x0 + dxdy * (start_point - e->y0));
			z->fx -= (float)(off_x);
			z->direction = (float)((e->invert) != 0 ? 1.0f : -1.0f);
			z->sy = (float)(e->y0);
			z->ey = (float)(e->y1);
			z->next = null;
			return z;
		}
	}
}
