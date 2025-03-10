#include "stb_image.h"
#include <iostream>
#include <fstream>
#include <vector>
#include <chrono>
#include <bitset>

inline bool is_transparent(stbi_uc* data, int width, int x, int y) {
	return data[4 * (y * width + x) + 3] == 0;
}

#define SCALE 1.0f
#define HALF_SCALE SCALE / 2.0f

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

void push_cube(std::vector<vertex>& vertices, std::vector<face>& faces, float width, float height, float x, float y, std::bitset< 4> obscuredFaces) {
	int startingIndex = vertices.size() + 1;

	// Front face
	vertices.emplace_back(vec3{ -HALF_SCALE + x, -HALF_SCALE + y, -HALF_SCALE }, vec3{ 0, 0, -1 }, vec2{ 0, 0 });
	vertices.emplace_back(vec3{ HALF_SCALE + x, -HALF_SCALE + y, -HALF_SCALE }, vec3{ 0, 0, -1 }, vec2{ 1, 0 });
	vertices.emplace_back(vec3{ HALF_SCALE + x, HALF_SCALE + y, -HALF_SCALE }, vec3{ 0, 0, -1 }, vec2{ 1, 1 });
	vertices.emplace_back(vec3{ -HALF_SCALE + x, HALF_SCALE + y, -HALF_SCALE }, vec3{ 0, 0, -1 }, vec2{ 0, 1 });
	faces.emplace_back(startingIndex, startingIndex + 1, startingIndex + 2, startingIndex + 3);

	// Back face
	vertices.emplace_back(vec3{ -HALF_SCALE + x, -HALF_SCALE + y, HALF_SCALE }, vec3{ 0, 0, 1 }, vec2{ 0, 0 });
	vertices.emplace_back(vec3{ HALF_SCALE + x, -HALF_SCALE + y, HALF_SCALE }, vec3{ 0, 0, 1 }, vec2{ 1, 0 });
	vertices.emplace_back(vec3{ HALF_SCALE + x, HALF_SCALE + y, HALF_SCALE }, vec3{ 0, 0, 1 }, vec2{ 1, 1 });
	vertices.emplace_back(vec3{ -HALF_SCALE + x, HALF_SCALE + y, HALF_SCALE }, vec3{ 0, 0, 1 }, vec2{ 0, 1 });
	faces.emplace_back(startingIndex + 4, startingIndex + 5, startingIndex + 6, startingIndex + 7);

	// Left face
	if (!obscuredFaces[0]) {
		vertices.emplace_back(vec3{ -HALF_SCALE + x, -HALF_SCALE + y, -HALF_SCALE }, vec3{ -1, 0, 0 }, vec2{ 0, 0 });
		vertices.emplace_back(vec3{ -HALF_SCALE + x, -HALF_SCALE + y, HALF_SCALE }, vec3{ -1, 0, 0 }, vec2{ 0, 1 });
		vertices.emplace_back(vec3{ -HALF_SCALE + x, HALF_SCALE + y, HALF_SCALE }, vec3{ -1, 0, 0 }, vec2{ 1, 1 });
		vertices.emplace_back(vec3{ -HALF_SCALE + x, HALF_SCALE + y, -HALF_SCALE }, vec3{ -1, 0, 0 }, vec2{ 1, 0 });
		faces.emplace_back(vertices.size() - 3, vertices.size() - 2, vertices.size() - 1, vertices.size() - 0);
	}

	// Right face
	if (!obscuredFaces[1]) {
		vertices.emplace_back(vec3{ HALF_SCALE + x, -HALF_SCALE + y, -HALF_SCALE }, vec3{ 1, 0, 0 }, vec2{ 0, 0 });
		vertices.emplace_back(vec3{ HALF_SCALE + x, HALF_SCALE + y, -HALF_SCALE }, vec3{ 1, 0, 0 }, vec2{ 1, 0 });
		vertices.emplace_back(vec3{ HALF_SCALE + x, HALF_SCALE + y, HALF_SCALE }, vec3{ 1, 0, 0 }, vec2{ 1, 1 });
		vertices.emplace_back(vec3{ HALF_SCALE + x, -HALF_SCALE + y, HALF_SCALE }, vec3{ 1, 0, 0 }, vec2{ 0, 1 });
		faces.emplace_back(vertices.size() - 3, vertices.size() - 2, vertices.size() - 1, vertices.size() - 0);
	}

	// Bottom face
	if (!obscuredFaces[2]) {
		vertices.emplace_back(vec3{ -HALF_SCALE + x, -HALF_SCALE + y, -HALF_SCALE }, vec3{ 0, -1, 0 }, vec2{ 0, 0 });
		vertices.emplace_back(vec3{ -HALF_SCALE + x, -HALF_SCALE + y, HALF_SCALE }, vec3{ 0, -1, 0 }, vec2{ 1, 0 });
		vertices.emplace_back(vec3{ HALF_SCALE + x, -HALF_SCALE + y, HALF_SCALE }, vec3{ 0, -1, 0 }, vec2{ 1, 1 });
		vertices.emplace_back(vec3{ HALF_SCALE + x, -HALF_SCALE + y, -HALF_SCALE }, vec3{ 0, -1, 0 }, vec2{ 0, 1 });
		faces.emplace_back(vertices.size() - 3, vertices.size() - 2, vertices.size() - 1, vertices.size() - 0);
	}

	// Top face
	if (!obscuredFaces[3]) {
		vertices.emplace_back(vec3{ -HALF_SCALE + x, HALF_SCALE + y, -HALF_SCALE }, vec3{ 0, 1, 0 }, vec2{ 0, 0 });
		vertices.emplace_back(vec3{ -HALF_SCALE + x, HALF_SCALE + y, HALF_SCALE }, vec3{ 0, 1, 0 }, vec2{ 1, 0 });
		vertices.emplace_back(vec3{ HALF_SCALE + x, HALF_SCALE + y, HALF_SCALE }, vec3{ 0, 1, 0 }, vec2{ 1, 1 });
		vertices.emplace_back(vec3{ HALF_SCALE + x, HALF_SCALE + y, -HALF_SCALE }, vec3{ 0, 1, 0 }, vec2{ 0, 1 });
		faces.emplace_back(vertices.size() - 3, vertices.size() - 2, vertices.size() - 1, vertices.size() - 0);
	}

	// Correct uvs
	for (int i = startingIndex; i < vertices.size(); ++i) {
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

	std::ofstream file;
	file.open(out);

	std::vector<vertex> vertices;
	std::vector<face>faces;

	for (int y = 0; y < height; y++) {
		for (int x = 0; x < width; x++) {
			if (is_transparent(data, width, x, y)) continue;

			std::bitset<4> obscuredFaces;
			obscuredFaces[0] = x > 0 && !is_transparent(data, width, x - 1, y);
			obscuredFaces[1] = x < width - 1 && !is_transparent(data, width, x + 1, y);
			obscuredFaces[2] = y > 0 && !is_transparent(data, width, x, y - 1);
			obscuredFaces[3] = y < height - 1 && !is_transparent(data, width, x, y + 1);
			push_cube(vertices, faces, width, height, x * SCALE, y * SCALE, obscuredFaces);
		}
	}

	for (vertex& v : vertices) {
		file << "v " << v.pos.x << " " << v.pos.y << " " << v.pos.z << "\n";
	}for (vertex& v : vertices) {
		file << "vn " << v.normal.x << " " << v.normal.y << " " << v.normal.z << "\n";
	}for (vertex& v : vertices) {
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