#include "stb_image.h"
#include <iostream>
#include <fstream>
#include <vector>
#include <chrono>
#include <bitset>
#include <array>

inline bool is_transparent(stbi_uc* data, int width, int x, int y) {
	return data[4 * (y * width + x) + 3] == 0;
}

#define SCALE 1.0f

struct vec3 {
	float x;
	float y;
	float z;
};

struct vec2 {
	float x;
	float y;
};

struct vertex {
	vec3 pos;
	vec3 normal;
	vec2 uv;

	vertex(vec3 pos, vec3 normal, vec2 uv) : pos(pos), normal(normal), uv(uv) {}
};

struct face {
	int indices[4];

	face(int a, int b, int c, int d) {
		indices[0] = a;
		indices[1] = b;
		indices[2] = c;
		indices[3] = d;
	}

	int& operator[](size_t index) {
		return indices[index];
	}

	const int& operator[](size_t index) const {
		return indices[index];
	}
};

void push_cube(std::vector<vertex>& vertices, std::vector<face>& faces, float width, float height, float x, float y, std::bitset< 4> obscuredFaces, bool greedyCulling, float greedyExpand) {
	int startingIndex = vertices.size() + 1;

	float scale = SCALE;


	if (!greedyCulling) {
		scale = SCALE * greedyExpand;
		// Front face
		//actually we don't need this face, and since the tex coords are broken on it we'll pretend the back face is the front face
		vertices.emplace_back(vec3{ x, y, 0 }, vec3{ 0, 0, -1 }, vec2{ 0, 0 });
		vertices.emplace_back(vec3{ scale + x, y, 0 }, vec3{ 0, 0, -1 }, vec2{ greedyExpand, 0 });
		vertices.emplace_back(vec3{ scale + x, scale + y, 0 }, vec3{ 0, 0, -1 }, vec2{ greedyExpand, greedyExpand });
		vertices.emplace_back(vec3{ x, scale + y, 0 }, vec3{ 0, 0, -1 }, vec2{ 0, greedyExpand });
	//	faces.emplace_back(vertices.size() - 3, vertices.size() - 2, vertices.size() - 1, vertices.size() - 0);

		// Back face
		vertices.emplace_back(vec3{ x, y, SCALE }, vec3{ 0, 0, 1 }, vec2{ 0, 0 });
		vertices.emplace_back(vec3{ scale + x, y, SCALE }, vec3{ 0, 0, 1 }, vec2{ greedyExpand, 0 });
		vertices.emplace_back(vec3{ scale + x, scale + y, SCALE }, vec3{ 0, 0, 1 }, vec2{ greedyExpand,greedyExpand });
		vertices.emplace_back(vec3{ x, scale + y, SCALE }, vec3{ 0, 0, 1 }, vec2{ 0, greedyExpand });
		faces.emplace_back(vertices.size() - 3, vertices.size() - 2, vertices.size() - 1, vertices.size() - 0);
	}

	// Left face
	scale = SCALE;
	if (!obscuredFaces[0]) {
		vertices.emplace_back(vec3{ x, y, 0 }, vec3{ -1, 0, 0 }, vec2{ 0, 0 });
		vertices.emplace_back(vec3{ x, y, scale }, vec3{ -1, 0, 0 }, vec2{ 0, 1 });
		vertices.emplace_back(vec3{ x, scale + y, scale }, vec3{ -1, 0, 0 }, vec2{ 1, 1 });
		vertices.emplace_back(vec3{ x, scale + y, 0 }, vec3{ -1, 0, 0 }, vec2{ 1, 0 });
		faces.emplace_back(vertices.size() - 3, vertices.size() - 2, vertices.size() - 1, vertices.size() - 0);
	}

	// Right face
	if (!obscuredFaces[1]) {
		vertices.emplace_back(vec3{ scale + x, y, 0 }, vec3{ 1, 0, 0 }, vec2{ 0, 0 });
		vertices.emplace_back(vec3{ scale + x, scale + y, 0 }, vec3{ 1, 0, 0 }, vec2{ 1, 0 });
		vertices.emplace_back(vec3{ scale + x, scale + y, scale }, vec3{ 1, 0, 0 }, vec2{ 1, 1 });
		vertices.emplace_back(vec3{ scale + x, y, scale }, vec3{ 1, 0, 0 }, vec2{ 0, 1 });
		faces.emplace_back(vertices.size() - 3, vertices.size() - 2, vertices.size() - 1, vertices.size() - 0);
	}

	// Bottom face
	if (!obscuredFaces[2]) {
		vertices.emplace_back(vec3{ x, y, 0 }, vec3{ 0, -1, 0 }, vec2{ 0, 0 });
		vertices.emplace_back(vec3{ x, y, scale }, vec3{ 0, -1, 0 }, vec2{ 1, 0 });
		vertices.emplace_back(vec3{ scale + x, y, scale }, vec3{ 0, -1, 0 }, vec2{ 1, 1 });
		vertices.emplace_back(vec3{ scale + x, y, 0 }, vec3{ 0, -1, 0 }, vec2{ 0, 1 });
		faces.emplace_back(vertices.size() - 3, vertices.size() - 2, vertices.size() - 1, vertices.size() - 0);
	}

	// Top face
	if (!obscuredFaces[3]) {
		vertices.emplace_back(vec3{ x, scale + y, 0 }, vec3{ 0, 1, 0 }, vec2{ 0, 0 });
		vertices.emplace_back(vec3{ x, scale + y, scale }, vec3{ 0, 1, 0 }, vec2{ 1, 0 });
		vertices.emplace_back(vec3{ scale + x, scale + y, scale }, vec3{ 0, 1, 0 }, vec2{ 1, 1 });
		vertices.emplace_back(vec3{ scale + x, scale + y, 0 }, vec3{ 0, 1, 0 }, vec2{ 0, 1 });
		faces.emplace_back(vertices.size() - 3, vertices.size() - 2, vertices.size() - 1, vertices.size() - 0);
	}

	// Correct uvs
	for (int i = 0; i < vertices.size(); ++i) {
		vec2& uv = vertices[i].uv;
		uv.x = (x + uv.x) / width;
		uv.y = (y + uv.y) / height;
	}

}

void generate_model(const char* path, const char* out) {
	stbi_set_flip_vertically_on_load(true);
	int width, height, numChannels;


	stbi_uc* data = stbi_load(path, &width, &height, &numChannels, 4);
	if (!data) {
		std::cerr << "Failed to load image: " << path << "\n";
	}

	std::vector<vertex> vertices;
	std::vector<face>faces;

	// Greedy meshing
	std::vector<bool> greedyCullingArray;
	greedyCullingArray.resize(width * height);

	std::vector<float> greedyExpandArray;
	greedyExpandArray.resize(width * height);

	for (int i = 0; i < greedyCullingArray.size(); ++i) {
		greedyCullingArray[i] = false;
		greedyExpandArray[i] = 1;
	}

	for (int y = 0; y < height; y++) {
		for (int x = 0; x < width; x++) {
			if (is_transparent(data, width, x, y)) continue;
			if (greedyCullingArray[x * width + y]) continue; // not accurate but it will not cause any bugs, and will optimise the code a bit (sometimes we'll be able to skip a greedy mesh)

			float  greedyExpand = 1.0f;

			// Expand this mesh
			for (int e = 1; e + x < width - 1 && e + y < height - 1; ++e) {
				bool canExpand = true;

				for (int ex = 1; ex <= e + 1 && canExpand; ex++) {
					canExpand = !is_transparent(data, width, x + ex, y + e);
				}
				for (int ey = 1; ey <= e && canExpand; ey++) {
					canExpand = !is_transparent(data, width, x + e, y + ey);
				}

				if (canExpand) {
					greedyExpand = e + 1;
				}
				else {
					break;
				}
			}

			greedyExpandArray[x * width + y] = greedyExpand;

			// Mark what we called
			for (int cx = 0; cx < ((int)greedyExpand) - 1; cx++) {
				for (int cy = 0; cy < ((int)greedyExpand) - 1; cy++) {
					int index = (x + cx) * width + y + cy;
					greedyCullingArray[index] = true;
				}
			}
			greedyCullingArray[x * width + y] = false; // it was false previously we know that because of the guard clause at top
		}
	}

	// Generate voxels
	for (int y = 0; y < height; y++) {
		for (int x = 0; x < width; x++) {
			if (is_transparent(data, width, x, y)) continue;

			std::bitset<4> obscuredFaces;
			obscuredFaces[0] = x > 0 && !is_transparent(data, width, x - 1, y);
			obscuredFaces[1] = x < width - 1 && !is_transparent(data, width, x + 1, y);
			obscuredFaces[2] = y > 0 && !is_transparent(data, width, x, y - 1);
			obscuredFaces[3] = y < height - 1 && !is_transparent(data, width, x, y + 1);

			push_cube(vertices, faces, width, height, x * SCALE, y * SCALE, obscuredFaces, greedyCullingArray[x * width + y], greedyExpandArray[x * width + y]);
		}
	}

	// Convert faces and vertices into .obj file
	std::ofstream file;
	file.open(out);

	for (vertex& v : vertices) {
		file << "v " << v.pos.x << " " << v.pos.y << " " << v.pos.z << "\n";
	}
	for (vertex& v : vertices) {
		file << "vn " << v.normal.x << " " << v.normal.y << " " << v.normal.z << "\n";
	}
	for (vertex& v : vertices) {
		file << "vt " << v.uv.x << " " << v.uv.y << "\n";
	}

	for (face& face : faces) {
		file << "f " << face[0] << "/" << face[0] << "/" << face[0]
			<< " " << face[1] << "/" << face[1] << "/" << face[1]
			<< " " << face[2] << "/" << face[2] << "/" << face[2]
			<< " " << face[3] << "/" << face[3] << "/" << face[3] << "\n";
	}

	file.close();
}

int main() {

	auto now = std::chrono::system_clock::now();
	generate_model("test.png", "test.obj");
	auto end = std::chrono::system_clock::now();
	std::cout << "Generated model in " << std::chrono::duration_cast<std::chrono::milliseconds>(end - now).count() << "ms.\n";
}