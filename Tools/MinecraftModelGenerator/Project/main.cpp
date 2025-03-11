#include "stb_image.h"
#include <iostream>
#include <fstream>
#include <vector>
#include <chrono>
#include <bitset>
#include <array>
#include <unordered_map>
#include <string>
#include <iomanip>
#include <filesystem>

inline bool is_transparent(stbi_uc* data, int width, int x, int y) {
	return data[4 * (y * width + x) + 3] < 200;
}

struct vec3 {
	float x;
	float y;
	float z;

	bool operator==(const vec3& other) const {
		return x == other.x && y == other.y && z == other.z;
	}

	vec3(float x, float y, float z) : x(x), y(y), z(z) {}
};

struct vec2 {
	float x;
	float y;

	bool operator==(const vec2& other) const {
		return x == other.x && y == other.y;
	}
};

struct vertex {
	vec3 pos;
	vec3 normal;
	vec2 uv;

	vertex(vec3 pos, vec3 normal, vec2 uv) : pos(pos), normal(normal), uv(uv) {}

	bool operator==(const vertex& other) const {
		return pos == other.pos && normal == other.normal && uv == other.uv;
	}
};

// boost https://stackoverflow.com/questions/2590677/how-do-i-combine-hash-values-in-c0x
inline size_t hash_combine(std::size_t hash, std::size_t otherHash)
{
	hash ^= otherHash + 0x9e3779b9 + (hash << 6) + (hash >> 2);
	return hash;
}

struct hash_vec3 {
	size_t operator()(const vec3& v) const {
		size_t h1 = std::hash<float>{}(v.x);
		size_t h2 = std::hash<float>{}(v.y);
		size_t h3 = std::hash<float>{}(v.z);

		return hash_combine(h1, hash_combine(h2, h3));
	}
};

struct hash_vec2 {
	size_t operator()(const vec2& v) const {
		size_t h1 = std::hash<float>{}(v.x);
		size_t h2 = std::hash<float>{}(v.y);

		return hash_combine(h1, h2);
	}
};

struct hash_vertex {
	size_t operator()(const vertex& v) const {
		size_t h1 = hash_vec3{}(v.pos);
		size_t h2 = hash_vec3{}(v.normal);
		size_t h3 = hash_vec2{}(v.uv);

		return hash_combine(h1, hash_combine(h2, h3));
	}
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

void push_cube(std::vector<vertex>& vertices, std::vector<face>& faces, float width, float height, float x, float y, std::bitset< 4> obscuredFaces,
	bool greedyCulling, float greedyExpand, float originalScale, float pixelX, float pixelY) {
	int startingIndex = vertices.size();
	float scale = originalScale;

	const float zScale = 0.1f;

	if (!greedyCulling) {
		scale = originalScale * greedyExpand;

		// Front face
		vertices.emplace_back(vec3{ x, y, zScale }, vec3{ 0, 0, 1 }, vec2{ 0, 0 });
		vertices.emplace_back(vec3{ scale + x, y, zScale }, vec3{ 0, 0, 1 }, vec2{ greedyExpand, 0 });
		vertices.emplace_back(vec3{ scale + x, scale + y, zScale }, vec3{ 0, 0, 1 }, vec2{ greedyExpand, greedyExpand });
		vertices.emplace_back(vec3{ x, scale + y, zScale }, vec3{ 0, 0, 1 }, vec2{ 0, greedyExpand });
		faces.emplace_back(vertices.size() - 3, vertices.size() - 2, vertices.size() - 1, vertices.size() - 0);
	}

	// Left face
	scale = originalScale;
	if (!obscuredFaces[0]) {
		vertices.emplace_back(vec3{ x, y, 0 }, vec3{ -1, 0, 0 }, vec2{ 0, 0 });
		vertices.emplace_back(vec3{ x, y, zScale }, vec3{ -1, 0, 0 }, vec2{ 0, 0 });
		vertices.emplace_back(vec3{ x, scale + y, zScale }, vec3{ -1, 0, 0 }, vec2{0,0 });
		vertices.emplace_back(vec3{ x, scale + y, 0 }, vec3{ -1, 0, 0 }, vec2{ 0, 0 });
		faces.emplace_back(vertices.size() - 3, vertices.size() - 2, vertices.size() - 1, vertices.size() - 0);
	}

	// Right face
	if (!obscuredFaces[1]) {
		vertices.emplace_back(vec3{ scale + x, y, 0 }, vec3{ 1, 0, 0 }, vec2{ 0, 0 });
		vertices.emplace_back(vec3{ scale + x, scale + y, 0 }, vec3{ 1, 0, 0 }, vec2{ 0, 0 });
		vertices.emplace_back(vec3{ scale + x, scale + y, zScale }, vec3{ 1, 0, 0 }, vec2{ 0,0 });
		vertices.emplace_back(vec3{ scale + x, y, zScale }, vec3{ 1, 0, 0 }, vec2{ 0, 0 });
		faces.emplace_back(vertices.size() - 3, vertices.size() - 2, vertices.size() - 1, vertices.size() - 0);
	}

	// Bottom face
	if (!obscuredFaces[2]) {
		vertices.emplace_back(vec3{ x, y, 0 }, vec3{ 0, -1, 0 }, vec2{ 0, 0 });
		vertices.emplace_back(vec3{ x, y, zScale }, vec3{ 0, -1, 0 }, vec2{ 0, 0 });
		vertices.emplace_back(vec3{ scale + x, y, zScale }, vec3{ 0, -1, 0 }, vec2{ 0,0 });
		vertices.emplace_back(vec3{ scale + x, y, 0 }, vec3{ 0, -1, 0 }, vec2{ 0, 0 });
		faces.emplace_back(vertices.size() - 3, vertices.size() - 2, vertices.size() - 1, vertices.size() - 0);
	}

	// Top face
	if (!obscuredFaces[3]) {
		vertices.emplace_back(vec3{ x, scale + y, 0 }, vec3{ 0, 1, 0 }, vec2{ 0, 0 });
		vertices.emplace_back(vec3{ x, scale + y, zScale }, vec3{ 0, 1, 0 }, vec2{ 0, 0 });
		vertices.emplace_back(vec3{ scale + x, scale + y, zScale }, vec3{ 0, 1, 0 }, vec2{ 0,0 });
		vertices.emplace_back(vec3{ scale + x, scale + y, 0 }, vec3{ 0, 1, 0 }, vec2{ 0, 0 });
		faces.emplace_back(vertices.size() - 3, vertices.size() - 2, vertices.size() - 1, vertices.size() - 0);
	}

	// Correct uvs and scale z
	for (int i = startingIndex; i < vertices.size(); ++i) {
		vec2& uv = vertices[i].uv;
		uv.x = (pixelX + uv.x) / width;
		uv.y = (pixelY + uv.y) / height;
	}

}

void generate_model(const char* path, const char* out) {
	stbi_set_flip_vertically_on_load(true);
	int width, height, numChannels;


	stbi_uc* data = stbi_load(path, &width, &height, &numChannels, 4);
	if (!data) {
		std::cerr << "Failed to load image: " << path << "\n";
		return;
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

	// Scale & center
	float scale = 3.0f / width;
	float startingX = -width * scale * 0.5;
	float startingY = -height * scale * 0.5;

	// Generate voxels
	for (int y = 0; y < height; y++) {
		for (int x = 0; x < width; x++) {
			if (is_transparent(data, width, x, y)) continue;

			std::bitset<4> obscuredFaces;
			obscuredFaces[0] = x > 0 && !is_transparent(data, width, x - 1, y);
			obscuredFaces[1] = x < width - 1 && !is_transparent(data, width, x + 1, y);
			obscuredFaces[2] = y > 0 && !is_transparent(data, width, x, y - 1);
			obscuredFaces[3] = y < height - 1 && !is_transparent(data, width, x, y + 1);

			push_cube(vertices, faces, width, height, startingX + x * scale, startingY + y * scale, obscuredFaces, greedyCullingArray[x * width + y],
				greedyExpandArray[x * width + y], scale, x, y);
		}
	}

	// Remove duplicate vertices
	std::unordered_map<vertex, int, hash_vertex> uniqueVertices;
	for (int i = 0; i < vertices.size(); ++i) {
		auto it = uniqueVertices.find(vertices[i]);
		if (it == uniqueVertices.end()) {
			uniqueVertices.insert(std::make_pair(vertices[i], i));
			continue;
		}
		// dupe vertex!

		// fix the faces since we're gonna be removing something
		for (auto& face : faces) {
			for (auto& j : face.indices) {
				// .obj indices are 1-based

				// first we remap to use the vertex we already found instead of the duplicate one
				if (j - 1 == i) {
					j = 1 + it->second;
				}

				// now everything past this vertex needs to be decremented since we're removing from the vector
				if (j - 1 > i) j--;
			}
		}

		// remove the dupe vertex
		vertices.erase(vertices.begin() + i);
		--i;
	}

	// Convert faces and vertices into .obj file
	std::ofstream file;
	file.open(out);
	file << std::fixed << std::setprecision(6);

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

		if (vertices[face[0] - 1].normal == vec3(0.f, 0.f, 1.f)) {
			if (vertices[face[0] - 1].uv == vertices[face[1] - 1].uv ) {
				int a = 23048;
			}
		}
	}

	file.close();
}

int main() {

	int num = 0;
	auto nowTotal = std::chrono::system_clock::now();

	std::filesystem::create_directory("input");
	std::filesystem::create_directory("output");

	for (const auto& entry : std::filesystem::directory_iterator("input")) {
		if (entry.is_regular_file()) {
			num++;

			std::string input_file = entry.path().string();
			std::string output_file = "output/" + entry.path().stem().string() + ".obj";

			auto modelStart = std::chrono::system_clock::now();

			if (input_file == "input/.gitignore") continue;
			generate_model(input_file.c_str(), output_file.c_str());

			auto modelEnd = std::chrono::system_clock::now();

			std::chrono::duration<double> modelDuration = modelEnd - modelStart;
			std::cout << "Model " << num << " generated in " << std::chrono::duration_cast<std::chrono::milliseconds>(modelDuration).count() << "ms.\n";
		}
	}

	auto endTotal = std::chrono::system_clock::now();

	std::cout << "Generated " << num << " model(s) in " << std::chrono::duration_cast<std::chrono::milliseconds>(endTotal - nowTotal).count() << "ms.\n";

	std::cout << "Press enter to exit the program.\n";
	std::cin.get();
}